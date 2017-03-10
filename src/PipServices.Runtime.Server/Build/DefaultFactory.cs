using PipServices.Runtime.Boot;
using PipServices.Runtime.Cache;
using PipServices.Runtime.Counters;
using PipServices.Runtime.Logs;

namespace PipServices.Runtime.Build
{
    /// <summary>
    ///     Component factory that contains registrations of standard runtime components.
    ///     This factory is typically used as a base for microservice factories.
    /// </summary>
    public class DefaultFactory : ComponentFactory
    {
        /// <summary>
        ///     The instance of default factory
        /// </summary>
        public static readonly DefaultFactory Instance = new DefaultFactory();

        public DefaultFactory()
        {
            Register(NullLogger.ClassDescriptor, typeof(NullLogger));
            Register(ConsoleLogger.ClassDescriptor, typeof(ConsoleLogger));
            Register(NullCounters.ClassDescriptor, typeof(NullCounters));
            Register(LogCounters.ClassDescriptor, typeof(LogCounters));
            Register(NullCache.ClassDescriptor, typeof(NullCache));
            Register(MemoryCache.ClassDescriptor, typeof(MemoryCache));
            Register(FileBootConfig.ClassDescriptor, typeof(FileBootConfig));

            Register(ServiceFabricLogger.ClassDescriptor, typeof(ServiceFabricLogger));
            Register(ServiceFabricCache.ClassDescriptor, typeof(ServiceFabricCache));
        }
    }
}