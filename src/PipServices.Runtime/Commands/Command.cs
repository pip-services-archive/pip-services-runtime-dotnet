using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;
using PipServices.Runtime.Validation;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Represents a command that implements a command pattern
    /// </summary>
    public class Command : ICommand
    {
        private readonly IComponent _component;
        private readonly Func<string, DynamicMap, CancellationToken, Task<object>> _function;
        private readonly Schema _schema;

        /// <summary>
        ///     Creates a command instance
        /// </summary>
        /// <param name="component">a component this command belongs to</param>
        /// <param name="name">the name of the command</param>
        /// <param name="schema">a validation schema for command arguments</param>
        /// <param name="function">an execution function to be wrapped into this command.</param>
        public Command(IComponent component, string name, Schema schema,
            Func<string, DynamicMap, CancellationToken, Task<object>> function)
        {
            if (name == null)
                throw new NullReferenceException("Command name is not set");
            if (function == null)
                throw new NullReferenceException("Command function is not set");

            _component = component;
            Name = name;
            _schema = schema;
            _function = function;
        }

        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Executes the command given specific arguments as an input.
        /// </summary>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">the command arguments</param>
        /// <returns>execution result</returns>
        public async Task<object> Execute(string correlationId, DynamicMap args, CancellationToken cancellationToken)
        {
            // Validate arguments
            if (_schema != null)
            {
                var errors = Validate(args);
                // Throw the 1st error
                if (errors.Count > 0)
                    throw errors[0];
            }

            // Call the function
            try
            {
                return await _function(correlationId, args, cancellationToken);
            }
                // Rethrow handled errors
            catch (MicroserviceError err)
            {
                err.WithCorrelationId(correlationId);
                throw err;
            }
                // Intercept unhandled errors
            catch (Exception ex)
            {
                throw new UnknownError(
                    _component,
                    "CommandFailed",
                    "Execution " + Name + " failed: " + ex
                    )
                    .WithDetails(Name)
                    .WithCorrelationId(correlationId)
                    .Wrap(ex);
            }
        }

        /// <summary>
        ///     Performs validation of the command arguments.
        /// </summary>
        /// <param name="args">the command arguments</param>
        /// <returns>a list of errors or empty list if validation was successful</returns>
        public IList<MicroserviceError> Validate(DynamicMap args)
        {
            // When schema is not defined, then skip validation
            if (_schema == null)
                return new List<MicroserviceError>();

            // ToDo: Complete implementation
            return new List<MicroserviceError>();
        }
    }
}