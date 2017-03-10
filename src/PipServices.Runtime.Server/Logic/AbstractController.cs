using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Logic
{
    /// <summary>
    ///     Abstract implementation for business logic controller.
    /// </summary>
    public abstract class AbstractController : AbstractBusinessLogic, IController
    {
        /// <summary>
        ///     Creates instance of abstract controller
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractController(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     Sets references to other microservice components to enable their
        ///     collaboration.It is recommended to locate necessary components
        ///     and cache their references to void performance hit during normal operations.
        ///     Linking can only be performed once after configuration
        ///     and will cause an exception when it is called second time or out of order.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="components">references to microservice components.</param>
        public override void Link(DynamicMap context, ComponentSet components)
        {
            base.Link(context, components);

            // Commented until we decide to use command pattern as everywhere
            // Until now the main method is to implement specific methods with instrumentation
            //AddIntercepter(new TracingIntercepter(_loggers, "Executing"));
            //AddIntercepter(new TimingIntercepter(_counters, "exec_time"));
        }

        /// <summary>
        ///     Does instrumentation of performed business method by counting elapsed time.
        /// </summary>
        /// <param name="correlationId">the unique id to identify distributed transaction</param>
        /// <param name="name">the name of called business method</param>
        /// <returns>ITiming instance to be called at the end of execution of the method.</returns>
        protected ITiming Instrument(string correlationId, string name)
        {
            Trace(correlationId, "Executing " + name + " method");
            return BeginTiming(name + ".exec_time");
        }
    }
}