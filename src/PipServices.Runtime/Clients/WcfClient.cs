using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Clients
{
    public abstract class WcfClient<T> : AbstractClient where T : class
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "endpoint.protocol", "tcp",
            "endpoint.host", "localhost"
            //"endpoint.port", 3000,
            );

        protected T Channel;

        protected WcfClient(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     Sets component configuration parameters and switches component
        ///     to 'Configured' state.The configuration is only allowed once
        ///     right after creation.Attempts to perform reconfiguration will
        ///     cause an exception.
        /// </summary>
        /// <param name="config">the component configuration parameters.</param>
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

            // Configure HTTP protocol
            if (protocol == "http")
            {
                var address = protocol + "://" + host + ":" + port;

                var binding = new WebHttpBinding();
                var endpoint = new EndpointAddress(address);
                var factory = new ChannelFactory<T>(binding, endpoint);
                factory.Endpoint.Behaviors.Add(new WebHttpBehavior());

                Channel = factory.CreateChannel();
            }
            // Configure TCP protocol
            else
            {
                var address = "net.tcp://" + host + ":" + port;

                var binding = new NetTcpBinding();
                var endpoint = new EndpointAddress(address);
                var factory = new ChannelFactory<T>(binding, endpoint);

                Channel = factory.CreateChannel();
            }

            base.Open();
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            // Close client
            if (Channel != null)
            {
                try
                {
                    ((IClientChannel) Channel).Close();
                }
                catch (Exception ex)
                {
                    Warn(null, "Failed while closing WCF client", ex);
                }
            }

            Channel = null;

            base.Close();
        }

        private Endpoint ResolveEndpoint()
        {
            var endpoints = _config.Endpoints;
            if (endpoints.Count == 0)
                throw new ConfigError(this, "NoEndpoint", "Service endpoint is not configured in the client");

            // Todo: Complete implementation
            var endpoint = endpoints[0];

            ValidateEndpoint(endpoint);
            return endpoint;
        }

        private void ValidateEndpoint(Endpoint endpoint)
        {
            // Check for type
            var protocol = endpoint.Protocol;
            if (!"http".Equals(protocol) && !"tcp".Equals(protocol))
                throw new ConfigError(this, "SupportedProtocol", "Protocol type is not supported by REST transport")
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