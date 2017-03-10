using System;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.SelfHost;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Services
{
    /// <summary>
    ///     Abstract REST Service that uses Web API implementation.
    ///     Since it has to be inherited from ApiController, IComponent
    ///     implementation is added to the top
    /// </summary>
    public abstract class RestService<T, I> : AbstractService
        where T : class, IHttpLogicController<I>, new()
        where I : IBusinessLogic
    {
        private readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "endpoint.type", "http",
            "endpoint.host", "0.0.0.0",
            //"endpoint.port", 3000,
            "options.request_max_size", 1024*1024,
            "options.connect_timeout", 60000,
            "options.debug", true
            );

        private HttpSelfHostServer _host;

        protected I Logic;

        protected RestService(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     Sets component configuration parameters and switches from component
        ///     to 'Configured' state.The configuration is only allowed once
        ///     right after creation.Attempts to perform reconfiguration will
        ///     cause an exception.
        /// </summary>
        /// <param name="config">the component configuration parameters.</param>
        public override void Configure(ComponentConfig config)
        {
            base.Configure(config.WithDefaults(DefaultConfig));
        }

        /// <summary>
        ///     Opens the component, performs initialization, opens connections
        ///     to external services and makes the component ready for operations.
        ///     Opening can be done multiple times: right after linking
        ///     or reopening after closure.
        /// </summary>
        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            var ep = ResolveEndpoint();
            var protocol = ep.Protocol;
            var host = ep.Host;
            var port = ep.Port;
            var address = protocol + "://" + host + ":" + port;

            var config = new HttpSelfHostConfiguration(address);

            // Use routes
            config.MapHttpAttributeRoutes();

            // Override dependency resolver to inject this service as a controller
            config.DependencyResolver = new WebApiControllerResolver<T, I>(config.DependencyResolver, Logic);

            config.Services.Replace(typeof(IExceptionHandler), new MicroserviceExceptionHandler());

            try
            {
                _host = new HttpSelfHostServer(config);
                _host.OpenAsync().Wait();
            }
            catch (Exception ex)
            {
                throw new ConnectionError(this, "OpenFailed", "Openning REST service failed")
                    .WithCause(ex);
            }

            base.Open();
        }

        /// <summary>
        ///     Closes the component and all open connections, performs deinitialization
        ///     steps.Closure can only be done from opened state.Attempts to close
        ///     already closed component or in wrong order will cause exception.
        /// </summary>
        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            if (_host != null)
            {
                // Eat exceptions
                try
                {
                    _host.CloseAsync().Wait();
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
            if (!"http".Equals(protocol))
                throw new ConfigError(this, "NotSupported", "Protocol type is not supported by REST transport")
                    .WithDetails(protocol);

            // Check for host
            if (endpoint.Host == null)
                throw new ConfigError(this, "NoHost", "No host is configured in REST transport");

            // Check for port
            if (endpoint.Port == 0)
                throw new ConfigError(this, "NoPort", "No port is configured in REST transport");
        }

        /// <summary>
        ///     Generates a string representation for this component
        /// </summary>
        /// <returns>a component descriptor in string format</returns>
        public override string ToString()
        {
            return _descriptor.ToString();
        }
    }
}