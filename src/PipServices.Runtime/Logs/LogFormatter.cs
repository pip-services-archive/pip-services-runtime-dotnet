using System;

namespace PipServices.Runtime.Logs
{
    public static class LogFormatter
    {
        public static string FormatLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return "FATAL";
                case LogLevel.Error:
                    return "ERROR";
                case LogLevel.Warn:
                    return "WARN";
                case LogLevel.Info:
                    return "INFO";
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Trace:
                    return "TRACE";
                default:
                    return "UNDEF";
            }
        }

        public static string FormatMessage(object[] message)
        {
            if (message == null || message.Length == 0) return "";
            if (message.Length == 1) return "" + message[0];

            var output = "" + message[0];

            for (var i = 1; i < message.Length; i++)
                output += "," + message[i];

            return output;
        }

        public static string Format(LogLevel level, object[] message)
        {
            return DateTime.Now.ToString("s")
                   + " " + FormatLevel(level)
                   + " " + FormatMessage(message);
        }
    }
}