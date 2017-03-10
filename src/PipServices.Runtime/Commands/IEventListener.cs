using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Listener for command events
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        ///     Notifies that event occured.
        /// </summary>
        /// <param name="e">Event reference</param>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">Command arguments</param>
        /// <param name="result">Execution result or <code>null</code> for failure.</param>
        void OnEvent(IEvent e, string correlationId, DynamicMap value);
    }
}