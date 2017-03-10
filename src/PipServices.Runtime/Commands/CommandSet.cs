using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Data;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Handles command registration and execution.
    ///     Enables intercepters to control or modify command behavior
    /// </summary>
    public class CommandSet
    {
        private readonly Dictionary<string, ICommand> _commandsByName = new Dictionary<string, ICommand>();
        private readonly Dictionary<string, IEvent> _eventsByName = new Dictionary<string, IEvent>();
        private readonly List<ICommandIntercepter> _intercepters = new List<ICommandIntercepter>();

        /// <summary>
        ///     Get all supported commands
        /// </summary>
        public List<ICommand> Commands { get; } = new List<ICommand>();

        /// <summary>
        ///     Get all supported events
        /// </summary>
        public List<IEvent> Events { get; } = new List<IEvent>();

        /// <summary>
        ///     Find a specific command by its name.
        /// </summary>
        /// <param name="name">the command name.</param>
        /// <returns>a command reference or <code>null</code> is nothing was found</returns>
        public ICommand FindCommand(string name)
        {
            ICommand command = null;
            _commandsByName.TryGetValue(name, out command);
            return command;
        }

        /// <summary>
        ///     Finds a specific event by its name
        /// </summary>
        /// <param name="name">the event name</param>
        /// <returns>an event reference or <code>null</code> is nothing was found</returns>
        public IEvent FindEvent(string name)
        {
            IEvent evt = null;
            _eventsByName.TryGetValue(name, out evt);
            return evt;
        }

        /// <summary>
        ///     Builds execution chain including all intercepters and the specified command.
        /// </summary>
        /// <param name="command">the command to build a chain.</param>
        private void BuildCommandChain(ICommand command)
        {
            var next = command;
            for (var i = _intercepters.Count - 1; i >= 0; i--)
            {
                next = new InterceptedCommand(_intercepters[i], next);
            }
            _commandsByName.Add(next.Name, next);
        }

        /// <summary>
        ///     Rebuilds execution chain for all registered commands.
        ///     This method is typically called when intercepters are changed.
        ///     Because of that it is more efficient to register intercepters
        ///     before registering commands (typically it will be done in abstract classes).
        ///     However, that performance penalty will be only once during creation time.
        /// </summary>
        private void RebuildAllCommandChains()
        {
            _commandsByName.Clear();
            foreach (var command in Commands)
            {
                BuildCommandChain(command);
            }
        }

        /// <summary>
        ///     Adds a command to the command set.
        /// </summary>
        /// <param name="command">a command instance to be added</param>
        public void AddCommand(ICommand command)
        {
            Commands.Add(command);
            BuildCommandChain(command);
        }

        /// <summary>
        ///     Adds a list of commands to the command set
        /// </summary>
        /// <param name="commands">a list of commands to be added</param>
        public void AddCommands(IEnumerable<ICommand> commands)
        {
            foreach (var command in commands)
            {
                AddCommand(command);
            }
        }

        /// <summary>
        ///     Adds an event to the command set.
        /// </summary>
        /// <param name="evt">an event instance to be added</param>
        public void AddEvent(IEvent evt)
        {
            Events.Add(evt);
            _eventsByName.Add(evt.Name, evt);
        }

        /// <summary>
        ///     Adds a ist of events to the command set
        /// </summary>
        /// <param name="events">a list of events to be added</param>
        public void AddEvents(IEnumerable<IEvent> events)
        {
            foreach (var evt in events)
            {
                AddEvent(evt);
            }
        }

        /// <summary>
        ///     Adds commands from another command set to this one
        /// </summary>
        /// <param name="commands">a commands set to add commands from</param>
        public void AddCommandSet(CommandSet commands)
        {
            foreach (var command in commands.Commands)
            {
                AddCommand(command);
            }

            foreach (var evt in commands.Events)
            {
                AddEvent(evt);
            }
        }

        /// <summary>
        ///     Adds intercepter to the command set.
        /// </summary>
        /// <param name="intercepter">an intercepter instance to be added.</param>
        public void AddIntercepter(ICommandIntercepter intercepter)
        {
            _intercepters.Add(intercepter);
            RebuildAllCommandChains();
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
            // Get command and throw error if it doesn't exist
            var cref = FindCommand(command);
            if (cref == null)
                throw new UnknownError("NoCommand", "Requested command does not exist")
                    .WithDetails(command);

            // Generate correlationId if it doesn't exist
            // Use short ids for now
            if (correlationId == null)
                correlationId = IdGenerator.Short();

            // Validate command arguments before execution and throw the 1st found error
            var errors = cref.Validate(args);
            if (errors.Count > 0)
                throw errors[0];

            // Execute the command.
            return await cref.Execute(correlationId, args, cancellationToken);
        }

        /// <summary>
        ///     Validates command arguments.
        /// </summary>
        /// <param name="command">the command name.</param>
        /// <param name="args">a list of command arguments.</param>
        /// <returns>a list of validation errors or empty list when arguments are valid.</returns>
        public IList<MicroserviceError> Validate(string command, DynamicMap args)
        {
            var cref = FindCommand(command);
            if (cref == null)
            {
                var errors = new List<MicroserviceError>();
                errors.Add(new UnknownError("NoCommand", "Requested command does not exist")
                    .WithDetails(command)
                    );
                return errors;
            }
            return cref.Validate(args);
        }

        /// <summary>
        ///     Adds listener to all events.
        /// </summary>
        /// <param name="listener">A listener to be added</param>
        public void AddListener(IEventListener listener)
        {
            foreach (var evt in Events)
            {
                evt.AddListener(listener);
            }
        }

        /// <summary>
        ///     Remove listener to all events.
        /// </summary>
        /// <param name="listener">A listener to be added</param>
        public void RemoveListener(IEventListener listener)
        {
            foreach (var evt in Events)
            {
                evt.RemoveListener(listener);
            }
        }

        /// <summary>
        ///     Notifies all listeners about the event.
        /// </summary>
        /// <param name="evt">Event name</param>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="value">Event value</param>
        public void Notify(string evt, string correlationId, DynamicMap value)
        {
            var e = FindEvent(evt);
            if (e != null)
            {
                e.Notify(correlationId, value);
            }
        }
    }
}