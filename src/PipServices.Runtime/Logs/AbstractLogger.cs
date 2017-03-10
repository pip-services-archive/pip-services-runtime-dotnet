using System;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Logs
{
    public abstract class AbstractLogger : AbstractComponent, ILogger
    {
        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "options.level", LogLevel.Info // Maximum output log level
            );

        protected LogLevel _level = LogLevel.Info;

        protected AbstractLogger(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);

            base.Configure(config.WithDefaults(DefaultConfig));

            _level = ParseLevel(_config.Options.Get("level"));
        }

        /// <summary>
        ///     The current level of details
        /// </summary>
        public LogLevel Level
        {
            get { return _level; }
        }

        /// <summary>
        ///     Writes a message to the log
        /// </summary>
        /// <param name="level">a log level - Fatal, Error, Warn, Info, Debug or Trace</param>
        /// <param name="component">a component name</param>
        /// <param name="correlationId">a correlationId</param>
        /// <param name="message">a message objects</param>
        public abstract void Log(LogLevel level, string component, string correlationId, object[] message);

        /// <summary>
        ///     Parses log level from configuration file
        /// </summary>
        /// <param name="level">log level value</param>
        /// <returns>parsed log level</returns>
        protected LogLevel ParseLevel(object level)
        {
            if (level == null) return LogLevel.Info;

            var strLevel = level.ToString().ToUpper();
            if (strLevel == "0" || strLevel == "NOTHING" || strLevel == "NONE")
                return LogLevel.None;
            if (strLevel == "1" || strLevel == "FATAL")
                return LogLevel.Fatal;
            if (strLevel == "2" || strLevel == "ERROR")
                return LogLevel.Error;
            if (strLevel == "3" || strLevel == "WARN" || strLevel == "WARNING")
                return LogLevel.Warn;
            if (strLevel == "4" || strLevel == "INFO")
                return LogLevel.Info;
            if (strLevel == "5" || strLevel == "DEBUG")
                return LogLevel.Debug;
            if (strLevel == "6" || strLevel == "TRACE")
                return LogLevel.Trace;
            return LogLevel.Info;
        }
    }
}