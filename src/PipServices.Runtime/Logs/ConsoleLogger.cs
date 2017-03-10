using System;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Logs
{
    public class ConsoleLogger : AbstractLogger
    {
        /// <summary>
        ///     Unique descriptor for the ConsoleLogger component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Logs, "pip-services-runtime-log", "console", "*"
            );

        public ConsoleLogger()
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
            if (_level < level) return;

            var output = LogFormatter.Format(level, message);
            if (correlationId != null)
                output += ", correlated to " + correlationId;

            if (level >= LogLevel.Fatal && level <= LogLevel.Warn)
                Console.Error.WriteLine(output);
            else Console.Out.WriteLine(output);
        }
    }
}