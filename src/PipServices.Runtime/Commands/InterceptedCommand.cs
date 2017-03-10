using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Interceptor wrapper to turn it into stackable command
    /// </summary>
    public class InterceptedCommand : ICommand
    {
        private readonly ICommandIntercepter _intercepter;
        private readonly ICommand _next;

        /// <summary>
        ///     Creates instance of intercepted command by chaining
        ///     intercepter with the next intercepter in the chain
        ///     or command at the end of the chain.
        /// </summary>
        /// <param name="intercepter">the intercepter reference.</param>
        /// <param name="next">the next intercepter or command in the chain.</param>
        public InterceptedCommand(ICommandIntercepter intercepter, ICommand next)
        {
            _intercepter = intercepter;
            _next = next;
        }

        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name
        {
            get { return _intercepter.GetName(_next); }
        }

        /// <summary>
        ///     Executes the command given specific arguments as an input.
        /// </summary>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">the command arguments</param>
        /// <returns>execution result</returns>
        public async Task<object> Execute(string correlationId, DynamicMap args, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _intercepter.Execute(_next, correlationId, args), cancellationToken);
        }

        /// <summary>
        ///     Performs validation of the command arguments.
        /// </summary>
        /// <param name="args">the command arguments</param>
        /// <returns>a list of errors or empty list if validation was successful</returns>
        public IList<MicroserviceError> Validate(DynamicMap args)
        {
            return _intercepter.Validate(_next, args);
        }
    }
}