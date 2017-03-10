using PipServices.Runtime.Config;

namespace PipServices.Runtime.Services
{
    /// <summary>
    ///     Abstract implementation for all API service components
    /// </summary>
    public abstract class AbstractService : AbstractComponent, IService
    {
        /// <summary>
        ///     Creates and initializes instance of the APIs service
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractService(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }
    }
}