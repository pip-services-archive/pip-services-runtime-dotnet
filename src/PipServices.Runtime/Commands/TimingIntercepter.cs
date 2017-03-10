using System.Collections.Generic;
using System.Threading;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Intercepter that times execution elapsed time.
    /// </summary>
    public class TimingIntercepter : ICommandIntercepter
    {
        private readonly ICounters _counters;
        private readonly string _suffix;

        /// <summary>
        ///     Creates instance of timing intercepter.
        /// </summary>
        /// <param name="counters">a reference to performance counters</param>
        /// <param name="suffix">a suffix to create a counter name as <command>.<suffix></param>
        public TimingIntercepter(ICounters counters, string suffix)
        {
            _counters = counters;
            _suffix = suffix != null ? suffix : "ExecTime";
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
            // Starting measuring elapsed time
            ITiming timing = null;
            if (_counters != null)
            {
                var name = command.Name + "." + _suffix;
                timing = _counters.BeginTiming(name);
            }

            try
            {
                var task = command.Execute(correlationId, args, CancellationToken.None);
                task.Wait();
                return task.Result;
            }
            finally
            {
                // Complete measuring elapsed time
                if (timing != null) timing.EndTiming();
            }
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