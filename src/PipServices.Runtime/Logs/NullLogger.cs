using PipServices.Runtime.Config;

namespace PipServices.Runtime.Logs
{
    public class NullLogger : AbstractLogger
    {
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Logs, "pip-services-runtime-log", "null", "*"
            );

        public NullLogger()
            : base(ClassDescriptor)
        {
        }

        /// <summary>
        ///     Writes a message to the log
        /// </summary>
        /// <param name="level">a log level - Fatal, Error, Warn, Info, Debug or Trace</param>
        /// <param name="component">a component name</param>
        /// <param name="correlationId">a correlationId</param>
        /// <param name="message">a message objects</param>
        public override void Log(LogLevel level, string component, string correlationId, object[] message)
        {
        }
    }
}