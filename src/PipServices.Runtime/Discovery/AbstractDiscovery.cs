using System.Collections.Generic;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Discovery
{
    /// <summary>
    ///     Abstract implementation for all discovery components
    /// </summary>
    public abstract class AbstractDiscovery : AbstractComponent, IDiscovery
    {
        /// <summary>
        ///     Creates and initializes instance of discovery component
        /// </summary>
        /// <param name="descriptor">the unique component descriptor</param>
        protected AbstractDiscovery(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     Register in discovery service endpoint where API service binds to.
        ///     The endpoint shall contain discovery name to locate the registration.
        ///     If it's not defined, the registration doesn't do anything.
        /// </summary>
        /// <param name="endpoint">the endpoint to be registered.</param>
        public abstract void Register(Endpoint endpoint);

        /// <summary>
        ///     Resolves one endpoint from the list of service endpoints
        ///     to be called by a client.
        /// </summary>
        /// <param name="endpoints">
        ///     a list of endpoints to be resolved from.
        ///     The list must contain at least one endpoint with discovery name.
        /// </param>
        /// <returns>a resolved endpoint.</returns>
        public abstract Endpoint Resolve(IList<Endpoint> endpoints);

        /// <summary>
        ///     Resolves a list of endpoints from to be called by a client.
        /// </summary>
        /// <param name="endpoints">
        ///     a list of endpoints to be resolved from.
        ///     The list must contain at least one endpoint with discovery name.
        /// </param>
        /// <returns>a list with resolved endpoints.</returns>
        public abstract IList<Endpoint> ResolveAll(IList<Endpoint> endpoints);
    }
}