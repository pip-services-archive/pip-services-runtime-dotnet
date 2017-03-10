using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Interface for components that implement microservice business logic: controllers or decorators.
    /// </summary>
    public interface IBusinessLogic : IComponent
    {
        /// <summary>
        ///     All supported commands
        /// </summary>
        IList<ICommand> Commands { get; }

        /// <summary>
        ///     Find a specific command by its name.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <returns>a found command or null if nothing was found</returns>
        ICommand FindCommand(string command);

        /// <summary>
        ///     Execute command by its name with specified arguments.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">a list of command arguments.</param>
        /// <returns>the execution result.</returns>
        Task<object> Execute(string command, string correlationId, DynamicMap args, CancellationToken cancellationToken);

        /// <summary>
        ///     Validates command arguments.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <param name="args">a list of command arguments.</param>
        /// <returns>a list of validation errors or empty list when arguments are valid.</returns>
        IList<MicroserviceError> Validate(string command, DynamicMap args);
    }
}