using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Services
{
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed
        )]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple
        )]
    public abstract class WcfService : AbstractService
    {
        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "endpoint.type", "tcp",
            "endpoint.host", "0.0.0.0",
            "options.request_max_size", 1024*1024,
            "options.connect_timeout", 60000,
            "options.debug", true
            );

        private readonly Type _serviceType;

        private ServiceHost _host;

        protected WcfService(ComponentDescriptor descriptor, Type serviceType)
            : base(descriptor)
        {
            _serviceType = serviceType;
        }

        public override void Configure(ComponentConfig config)
        {
            base.Configure(config.WithDefaults(DefaultConfig));
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            var ep = ResolveEndpoint();
            var protocol = ep.Protocol;
            var host = ep.Host;
            var port = ep.Port;

            try
            {
                _host = new ServiceHost(this);

                // Configure HTTP protocol
                if (protocol == "http")
                {
                    var address = protocol + "://" + host + ":" + port;

                    var binding = new WebHttpBinding();
                    var endpoint = _host.AddServiceEndpoint(_serviceType, binding, address);

                    var behavior = new WebHttpBehavior
                    {
                        AutomaticFormatSelectionEnabled = false,
                        DefaultOutgoingRequestFormat = WebMessageFormat.Json,
                        DefaultOutgoingResponseFormat = WebMessageFormat.Json,
                        FaultExceptionEnabled = false
                    };
                    endpoint.EndpointBehaviors.Add(behavior);
                }
                // Configure default TCP binary protocol
                else
                {
                    var address = "net.tcp://" + host + ":" + port;

                    var binding = new NetTcpBinding();
                    var endpoint = _host.AddServiceEndpoint(_serviceType, binding, address);
                }

                // Todo: When this is enabled Fault contract may not work
                var debugBehavior = _host.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (debugBehavior == null)
                {
                    debugBehavior = new ServiceDebugBehavior();
                    _host.Description.Behaviors.Add(debugBehavior);
                }
                debugBehavior.IncludeExceptionDetailInFaults = true;

                try
                {
                    _host.Open();
                }
                catch (Exception ex)
                {
                    throw new ConnectionError(this, "OpenFailed", "Openning WCF service failed")
                        .WithCause(ex);
                }

                base.Open();
            }
            catch (Exception ex)
            {
                _host = null;

                throw new ConnectionError(this, "ConnectFailed", "Openning WCF service failed")
                    .WithCause(ex);
            }
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            if (_host != null)
            {
                // Eat exceptions
                try
                {
                    _host.Close();
                }
                catch (Exception ex)
                {
                    Warn(null, "Failed while closing REST service", ex);
                }

                _host = null;
            }

            base.Close();
        }

        private Endpoint ResolveEndpoint()
        {
            // Todo: Complete implementation
            var address = _config.Endpoint;
            ValidateAddress(address);
            return address;
        }

        private void ValidateAddress(Endpoint endpoint)
        {
            // Check for type
            var protocol = endpoint.Protocol;
            if (!"http".Equals(protocol) && !"tcp".Equals(protocol))
                throw new ConfigError(this, "NotSupported", "Protocol type is not supported by REST transport")
                    .WithDetails(protocol);

            // Check for host
            if (endpoint.Host == null)
                throw new ConfigError(this, "NoHost", "No host is configured in REST transport");

            // Check for port
            if (endpoint.Port == 0)
                throw new ConfigError(this, "NoPort", "No port is configured in REST transport");
        }
    }
}