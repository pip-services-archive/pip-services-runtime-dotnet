using System.Collections.Generic;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Interface for stackable command intercepters
    /// </summary>
    public interface ICommandIntercepter
    {
        /// <summary>
        ///     Gets the command name. Intercepter can modify the name if needed
        /// </summary>
        /// <param name="command">the name of intercepted command</param>
        /// <returns>the command name</returns>
        string GetName(ICommand command);

        /// <summary>
        ///     Executes the command given specific arguments as an input.
        /// </summary>
        /// <param name="command">the intercepted command</param>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">map with command arguments</param>
        /// <returns>execution result</returns>
        object Execute(ICommand command, string correlationId, DynamicMap args);

        /// <summary>
        ///     Performs validation of the command arguments.
        /// </summary>
        /// <param name="command">the intercepted command</param>
        /// <param name="args">map with command arguments</param>
        /// <returns>a list of errors or empty list if validation was successful.</returns>
        IList<MicroserviceError> Validate(ICommand command, DynamicMap args);
    }
}