using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Commands;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Logic
{
    /// <summary>
    ///     Abstract implementation for all microservice business logic components
    ///     that are able to perform business functions(commands).
    /// </summary>
    public class AbstractBusinessLogic : AbstractComponent, IBusinessLogic
    {
        private readonly CommandSet _commands = new CommandSet();

        /// <summary>
        ///     Creates instance of abstract functional component
        /// </summary>
        /// <param name="descriptor">the unique descriptor that is used to identify and locate the component.</param>
        protected AbstractBusinessLogic(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        /// <summary>
        ///     All supported commands
        /// </summary>
        public IList<ICommand> Commands
        {
            get { return _commands.Commands; }
        }

        /// <summary>
        ///     Find a specific command by its name.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <returns>a found command or null if nothing was found</returns>
        public ICommand FindCommand(string command)
        {
            return _commands.FindCommand(command);
        }

        /// <summary>
        ///     Execute command by its name with specified arguments.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="args">a list of command arguments.</param>
        /// <returns>the execution result.</returns>
        public async Task<object> Execute(string command, string correlationId, DynamicMap args,
            CancellationToken cancellationToken)
        {
            return await _commands.Execute(command, correlationId, args, cancellationToken);
        }

        /// <summary>
        ///     Validates command arguments.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <param name="args">a list of command arguments.</param>
        /// <returns>a list of validation errors or empty list when arguments are valid.</returns>
        public IList<MicroserviceError> Validate(string command, DynamicMap args)
        {
            return _commands.Validate(command, args);
        }

        /// <summary>
        ///     Adds a command to the command set.
        /// </summary>
        /// <param name="command">a command instance to be added</param>
        protected void AddCommand(ICommand command)
        {
            _commands.AddCommand(command);
        }

        /// <summary>
        ///     Adds commands from another command set to this one.
        /// </summary>
        /// <param name="commands">a command set that contains commands to be added</param>
        protected void AddCommandSet(CommandSet commands)
        {
            _commands.AddCommandSet(commands);
        }

        /// <summary>
        ///     Delegates all commands to another functional component.
        /// </summary>
        /// <param name="component">a functional component to perform delegated commands.</param>
        protected void DelegateCommands(IBusinessLogic component)
        {
            _commands.AddCommands(component.Commands);
        }

        /// <summary>
        ///     Adds intercepter to the command set.
        /// </summary>
        /// <param name="interceptor">an intercepter instance to be added.</param>
        protected void AddIntercepter(ICommandIntercepter interceptor)
        {
            _commands.AddIntercepter(interceptor);
        }
    }
}