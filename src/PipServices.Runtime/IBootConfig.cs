using PipServices.Runtime.Config;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Interface for microservice component responsible for
    ///     reading bootstrap microservice configuration.
    ///     It is still not clear if that logic shall be in component or
    ///     separate BootstrapConfig classes.
    /// </summary>
    public interface IBootConfig : IComponent
    {
        /// <summary>
        ///     Reads microservice configuration from the source
        /// </summary>
        /// <returns>a microservice configuration</returns>
        MicroserviceConfig ReadConfig();
    }
}