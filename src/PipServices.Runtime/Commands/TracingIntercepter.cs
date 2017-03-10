using System.Collections.Generic;
using System.Threading;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Intercepter that writes trace messages for every executed command.
    /// </summary>
    public class TracingIntercepter : ICommandIntercepter
    {
        private readonly IList<ILogger> _loggers;
        private readonly string _verb;

        /// <summary>
        ///     Creates instance of tracing intercepter
        /// </summary>
        /// <param name="loggers">a list of logger components.</param>
        /// <param name="verb">
        ///     a verb for tracing message as '
        ///     <verb>
        ///         <command>, ...'
        /// </param>
        public TracingIntercepter(IList<ILogger> loggers, string verb)
        {
            _loggers = loggers;
            _verb = verb != null ? verb : "Executing";
        }

        /// <summary>
        ///     Gets the command name. Intercepter can modify the name if needed
        /// </summary>
        /// <param name="command">the name of intercepted command</param>
        /// <returns>the command name</returns>
        public string GetName(ICommand command)
        {
            return command.Name;
        }

        /// <summary>
        ///     Executes the command given specific arguments as an input.
        /// </summary>
        /// <param name="command">the intercepted command</param>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">map with command arguments</param>
        /// <returns>execution result</returns>
        public object Execute(ICommand command, string correlationId, DynamicMap args)
        {
            // Write trace message about the command execution
            if (_loggers != null && _loggers.Count > 0)
            {
                var name = command.Name;
                var message = _verb + " " + name + " command";
                if (correlationId != null)
                    message += ", correlated to " + correlationId;

                foreach (var logger in _loggers)
                    logger.Log(LogLevel.Trace, null, correlationId, new object[] {message});
            }

            var task = command.Execute(correlationId, args, CancellationToken.None);
            task.Wait();
            return task.Result;
        }

        /// <summary>
        ///     Performs validation of the command arguments.
        /// </summary>
        /// <param name="command">the intercepted command</param>
        /// <param name="args">map with command arguments</param>
        /// <returns>a list of errors or empty list if validation was successful.</returns>
        public IList<MicroserviceError> Validate(ICommand command, DynamicMap args)
        {
            return command.Validate(args);
        }
    }
}