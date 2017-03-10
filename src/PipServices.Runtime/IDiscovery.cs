using System.Collections.Generic;
using PipServices.Runtime.Config;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Service discovery component used to register addresses of the microservice
    ///     service endpoints or to resolve addresses of external services called by clients.
    /// </summary>
    public interface IDiscovery : IComponent
    {
        /// <summary>
        ///     Register in discovery service endpoint where API service binds to.
        ///     The endpoint shall contain discovery name to locate the registration.
        ///     If it's not defined, the registration doesn't do anything.
        /// </summary>
        /// <param name="endpoint">the endpoint to be registered.</param>
        void Register(Endpoint endpoint);

        /// <summary>
        ///     Resolves one endpoint from the list of service endpoints
        ///     to be called by a client.
        /// </summary>
        /// <param name="endpoints">
        ///     a list of endpoints to be resolved from.
        ///     The list must contain at least one endpoint with discovery name.
        /// </param>
        /// <returns>a resolved endpoint.</returns>
        Endpoint Resolve(IList<Endpoint> endpoints);

        /// <summary>
        ///     Resolves a list of endpoints from to be called by a client.
        /// </summary>
        /// <param name="endpoints">
        ///     a list of endpoints to be resolved from.
        ///     The list must contain at least one endpoint with discovery name.
        /// </param>
        /// <returns>a list with resolved endpoints.</returns>
        IList<Endpoint> ResolveAll(IList<Endpoint> endpoints);
    }
}