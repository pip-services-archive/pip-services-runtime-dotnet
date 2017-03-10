namespace PipServices.Runtime
{
    /// <summary>
    ///     Logger that logs messages from other microservice components.
    /// </summary>
    public interface ILogger : IComponent
    {
        /// <summary>
        ///     The current level of details
        /// </summary>
        LogLevel Level { get; }

        /// <summary>
        ///     Writes a message to the log
        /// </summary>
        /// <param name="level">a log level - Fatal, Error, Warn, Info, Debug or Trace</param>
        /// <param name="component">a component name</param>
        /// <param name="correlationId">a correlationId</param>
        /// <param name="message">a message objects</param>
        void Log(LogLevel level, string component, string correlationId, object[] message);
    }
}