namespace PipServices.Runtime
{
    /// <summary>
    ///     Logging levels to determine details of logged messages
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        ///     Nothing to be logged
        /// </summary>
        None = 0,

        /// <summary>
        ///     Logs only fatal errors that cause microservice to fail
        /// </summary>
        Fatal = 1,

        /// <summary>
        ///     Logs all errors - fatal or recoverable
        /// </summary>
        Error = 2,

        /// <summary>
        ///     Logs errors and warnings
        /// </summary>
        Warn = 3,

        /// <summary>
        ///     Logs errors and important information messages
        /// </summary>
        Info = 4,

        /// <summary>
        ///     Logs everything up to high-level debugging information
        /// </summary>
        Debug = 5,

        /// <summary>
        ///     Logs everything down to fine-granular debugging messages
        /// </summary>
        Trace = 6
    }
}