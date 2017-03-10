using PipServices.Runtime.Config;

namespace PipServices.Runtime.Clients
{
    /// <summary>
    ///     Abstract implementation for all microservice client components.
    /// </summary>
    public abstract class AbstractClient : AbstractComponent, IClient
    {
        /// <summary>
        ///     Creates and initializes instance of the microservice client component.
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractClient(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     Does instrumentation of performed business method by counting elapsed time.
        /// </summary>
        /// <param name="correlationId">the unique id to identify distributed transaction</param>
        /// <param name="name">the name of called business method</param>
        /// <returns>ITiming instance to be called at the end of execution of the method.</returns>
        protected ITiming Instrument(string correlationId, string name)
        {
            Trace(correlationId, "Calling " + name + " method");
            return BeginTiming(name + ".call_time");
        }
    }
}