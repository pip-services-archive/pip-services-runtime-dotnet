using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Interface for commands that execute functional operations.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Executes the command given specific arguments as an input.
        /// </summary>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">the command arguments</param>
        /// <returns>execution result</returns>
        Task<object> Execute(string correlationId, DynamicMap args, CancellationToken cancellationToken);

        /// <summary>
        ///     Performs validation of the command arguments.
        /// </summary>
        /// <param name="args">the command arguments</param>
        /// <returns>a list of errors or empty list if validation was successful</returns>
        IList<MicroserviceError> Validate(DynamicMap args);
    }
}