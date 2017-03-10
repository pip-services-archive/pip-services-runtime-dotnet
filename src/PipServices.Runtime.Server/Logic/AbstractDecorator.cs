using PipServices.Runtime.Config;

namespace PipServices.Runtime.Logic
{
    /// <summary>
    ///     Abstract implementation of business logic decorators.
    ///     Decorators are typically used to alter standard behavior
    ///     of microservice business logic by injecting custom logic
    ///     before or after execution.
    /// </summary>
    public abstract class AbstractDecorator : AbstractBusinessLogic, IDecorator
    {
        /// <summary>
        ///     Creates instance of abstract business logic decorator
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractDecorator(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }
    }
}