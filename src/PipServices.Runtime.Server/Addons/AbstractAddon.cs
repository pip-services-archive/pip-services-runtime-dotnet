using PipServices.Runtime.Config;

namespace PipServices.Runtime.Addons
{
    /// <summary>
    ///     Abstract implementation for microservice addons.
    /// </summary>
    public abstract class AbstractAddon : AbstractComponent, IAddon
    {
        /// <summary>
        ///     Creates and initializes instance of the microservice addon
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractAddon(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }
    }
}