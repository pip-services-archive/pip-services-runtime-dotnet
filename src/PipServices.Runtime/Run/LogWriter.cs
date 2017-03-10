using System;
using System.Collections.Generic;
using PipServices.Runtime.Logs;

namespace PipServices.Runtime.Run
{
    public static class LogWriter
    {
        private static void Log(IEnumerable<IComponent> components, LogLevel level,
            string component, string correlationId, object[] message)
        {
            var logged = false;

            // Output to all loggers
            if (components != null)
            {
                foreach (var cref in components)
                {
                    if (cref is ILogger)
                    {
                        var logger = (ILogger) cref;
                        logger.Log(level, component, correlationId, message);
                        logged = logged || logger.Descriptor.Type == "console";
                    }
                }
            }

            // If nothing was logged then write to console
            if (logged == false)
            {
                var output = LogFormatter.Format(level, message);
                if (correlationId != null)
                    output += ", correlated to " + correlationId;

                if (level >= LogLevel.Fatal && level <= LogLevel.Warn)
                    Console.Error.WriteLine(output);
                else Console.Out.WriteLine(output);
            }
        }

        public static void Fatal(IEnumerable<IComponent> components, params object[] message)
        {
            Log(components, LogLevel.Fatal, null, null, message);
        }

        public static void Error(IEnumerable<IComponent> components, params object[] message)
        {
            Log(components, LogLevel.Error, null, null, message);
        }

        public static void Warn(IEnumerable<IComponent> components, params object[] message)
        {
            Log(components, LogLevel.Warn, null, null, message);
        }

        public static void Info(IEnumerable<IComponent> components, params object[] message)
        {
            Log(components, LogLevel.Info, null, null, message);
        }

        public static void Debug(IEnumerable<IComponent> components, params object[] message)
        {
            Log(components, LogLevel.Debug, null, null, message);
        }

        public static void Trace(IEnumerable<IComponent> components, params object[] message)
        {
            Log(components, LogLevel.Trace, null, null, message);
        }
    }
}