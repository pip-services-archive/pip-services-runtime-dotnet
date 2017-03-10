using System.Collections.Generic;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Base
{
    /// <summary>
    ///     Component log helper
    /// </summary>
    public class ComponentLogWriter
    {
        private readonly IComponent _component;
        private readonly IList<ILogger> _loggers = new List<ILogger>();

        /// <summary>
        ///     Constructs log helper for components.
        /// </summary>
        /// <param name="component">Component reference</param>
        /// <param name="otherComponents">References to other components</param>
        public ComponentLogWriter(IComponent component, ComponentSet otherComponents)
        {
            _component = component;

            // Get reference to logger(s)
            var loggers = otherComponents.GetAllOptional(
                new ComponentDescriptor(Category.Logs, null, null, null)
                );
            _loggers.Clear();
            foreach (var logger in loggers)
            {
                _loggers.Add((ILogger) logger);
            }
        }

        /// <summary>
        ///     Writes message to log
        /// </summary>
        /// <param name="level">a message logging level</param>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a message objects</param>
        public void WriteLog(LogLevel level, string correlationId, object[] message)
        {
            if (_loggers == null || _loggers.Count == 0)
                return;

            var component = _component != null ? _component.ToString() : null;
            foreach (var logger in _loggers)
            {
                logger.Log(level, component, correlationId, message);
            }
        }

        /// <summary>
        ///     Logs fatal error that causes microservice to shutdown
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public void Fatal(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Fatal, null, message);
        }

        /// <summary>
        ///     Logs recoverable error
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public void Error(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Error, null, message);
        }

        /// <summary>
        ///     Logs warning messages
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public void Warn(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Warn, null, message);
        }

        /// <summary>
        ///     Logs important information message
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public void Info(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Info, null, message);
        }

        /// <summary>
        ///     Logs high-level debugging messages
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public void Debug(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Debug, null, message);
        }

        /// <summary>
        ///     Logs fine-granular debugging message
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public void Trace(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Trace, null, message);
        }
    }
}