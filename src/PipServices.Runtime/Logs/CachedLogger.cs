using System;
using System.Collections.Generic;
using System.Threading;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Logs
{
    public abstract class CachedLogger : AbstractLogger
    {
        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "options.timeout", 1000 // timeout in milliseconds
            );

        private List<LogEntry> _cache = new List<LogEntry>();
        private Timer _interval;

        protected CachedLogger(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);

            base.Configure(config.WithDefaults(DefaultConfig));
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            // Define dump timeout 
            var timeout = Math.Max(1000, _config.Options.GetInteger("timeout"));

            // Stop previously set timer 
            if (_interval != null)
                _interval.Dispose();

            // Set a new timer
            _interval = new Timer(PeriodicSave, null, timeout, timeout);

            base.Open();
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            // Stop previously set timer
            if (_interval != null)
            {
                _interval.Dispose();

                // Clear timer ID
                _interval = null;
            }

            PeriodicSave();

            base.Close();
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
            if (_level >= level)
                _cache.Add(new LogEntry(level, component, correlationId, message));
        }

        private void PeriodicSave(object state = null)
        {
            if (_cache.Count == 0) return;

            var entries = _cache;
            _cache = new List<LogEntry>();
            Save(entries);
        }

        protected abstract void Save(List<LogEntry> entries);
    }
}