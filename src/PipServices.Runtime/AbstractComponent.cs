using System;
using System.Collections.Generic;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Abstract implementation for all microservice components.
    /// </summary>
    public abstract class AbstractComponent : IComponent
    {
        protected ComponentConfig _config;
        protected ICounters _counters;
        protected ComponentDescriptor _descriptor;
        protected IDiscovery _discovery;
        protected IList<ILogger> _loggers = new List<ILogger>();
        protected State _state;

        public DynamicMap Context { get; private set; }

        protected AbstractComponent(ComponentDescriptor descriptor)
        {
            if (descriptor == null)
                throw new NullReferenceException("Descriptor is not set");

            _descriptor = descriptor;
            _state = State.Initial;
        }

        /// <summary>
        ///     Gets the unique component descriptor that can identify
        ///     and locate the component inside the microservice.
        /// </summary>
        public ComponentDescriptor Descriptor
        {
            get { return _descriptor; }
        }

        /* Life cycle management */

        /// <summary>
        ///     Gets the current state of the component.
        /// </summary>
        public State State
        {
            get { return _state; }
        }

        /// <summary>
        ///     Sets component configuration parameters and switches from component
        ///     to 'Configured' state.The configuration is only allowed once
        ///     right after creation.Attempts to perform reconfiguration will
        ///     cause an exception.
        /// </summary>
        /// <param name="config">the component configuration parameters.</param>
        public virtual void Configure(ComponentConfig config)
        {
            CheckNewStateAllowed(State.Configured);
            _config = config;
            _state = State.Configured;
        }

        /// <summary>
        ///     Sets references to other microservice components to enable their
        ///     collaboration.It is recommended to locate necessary components
        ///     and cache their references to void performance hit during
        ///     normal operations.
        ///     Linking can only be performed once after configuration
        ///     and will cause an exception when it is called second time
        ///     or out of order.
        /// </summary>
        /// <param name="context">Application context</param>
        /// <param name="components">References to microservice components.</param>
        public virtual void Link(DynamicMap context, ComponentSet components)
        {
            CheckNewStateAllowed(State.Linked);

            Context = context;

            // Get reference to discovery component
            _discovery = (IDiscovery) components.GetOneOptional(
                new ComponentDescriptor(Category.Discovery, null, null, null)
                );

            // Get reference to logger(s)
            var loggers = components.GetAllOptional(
                new ComponentDescriptor(Category.Logs, null, null, null)
                );
            _loggers.Clear();
            foreach (var logger in loggers)
            {
                _loggers.Add((ILogger) logger);
            }

            // Get reference to counters component
            _counters = (ICounters) components.GetOneOptional(
                new ComponentDescriptor(Category.Counters, null, null, null)
                );

            _state = State.Linked;
        }

        /// <summary>
        ///     Change state of component to Opening
        /// </summary>
        protected virtual void StartOpening()
        {
            CheckNewStateAllowed(State.Opening);
            _state = State.Opening;
            Trace(null, "Component " + _descriptor + " opening");
        }

        /// <summary>
        ///     Opens the component, performs initialization, opens connections
        ///     to external services and makes the component ready for operations.
        ///     Opening can be done multiple times: right after linking
        ///     or reopening after closure.
        /// </summary>
        public virtual void Open()
        {
            CheckNewStateAllowed(State.Opened);
            _state = State.Opened;
            Trace(null, "Component " + _descriptor + " opened");
        }

        /// <summary>
        ///     Closes the component and all open connections, performs deinitialization
        ///     steps.Closure can only be done from opened state.Attempts to close
        ///     already closed component or in wrong order will cause exception.
        /// </summary>
        public virtual void Close()
        {
            CheckNewStateAllowed(State.Closed);
            _state = State.Closed;
            Trace(null, "Component " + _descriptor + " closed");
        }

        /// <summary>
        ///     Checks if specified state matches to the current one.
        ///     It throws an exception if states don't match.
        /// </summary>
        /// <param name="state">The expected state.</param>
        protected void CheckCurrentState(State state)
        {
            if (_state != state)
                throw new StateError(this, "InvalidState", "Component is in wrong state")
                    .WithDetails(_state, state);
        }

        /// <summary>
        ///     Checks if transition to the specified state is allowed from the current one.
        ///     It throws an exception when transition is not allowed.
        /// </summary>
        /// <param name="newState">the new state to make transition</param>
        protected void CheckNewStateAllowed(State newState)
        {
            if (newState == State.Configured && _state != State.Initial)
                throw new StateError(this, "InvalidState", "Component cannot be configured")
                    .WithDetails(_state, State.Configured);

            if (newState == State.Linked && _state != State.Configured)
                throw new StateError(this, "InvalidState", "Component cannot be linked")
                    .WithDetails(_state, State.Linked);

            if (newState == State.Opening && _state != State.Linked && _state != State.Closed)
                throw new StateError(this, "InvalidState", "Component cannot be opened")
                    .WithDetails(_state, State.Opening);

            if (newState == State.Opened && _state != State.Opening && _state != State.Linked && _state != State.Closed)
                throw new StateError(this, "InvalidState", "Component cannot be opened")
                    .WithDetails(_state, State.Opened);

            if (newState == State.Closed && _state != State.Opened)
                throw new StateError(this, "InvalidState", "Component cannot be closed")
                    .WithDetails(_state, State.Closed);
        }

        /* Logging */

        /// <summary>
        ///     Writes message to log
        /// </summary>
        /// <param name="level">a message logging level</param>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a message objects</param>
        protected void WriteLog(LogLevel level, string correlationId, object[] message)
        {
            if (_loggers == null || _loggers.Count == 0)
                return;

            var component = _descriptor.ToString();
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
        public virtual void Fatal(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Fatal, null, message);
        }

        /// <summary>
        ///     Logs recoverable error
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public virtual void Error(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Error, null, message);
        }

        /// <summary>
        ///     Logs warning messages
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public virtual void Warn(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Warn, null, message);
        }

        /// <summary>
        ///     Logs important information message
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public virtual void Info(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Info, null, message);
        }

        /// <summary>
        ///     Logs high-level debugging messages
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public virtual void Debug(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Debug, null, message);
        }

        /// <summary>
        ///     Logs fine-granular debugging message
        /// </summary>
        /// <param name="correlationId">a unique id to identify distributed transaction</param>
        /// <param name="message">a list with message values</param>
        public virtual void Trace(string correlationId, params object[] message)
        {
            WriteLog(LogLevel.Trace, null, message);
        }

        /// <summary>
        ///     Starts measurement of execution time interval.
        ///     The method returns ITiming object that provides endTiming()
        ///     method that shall be called when execution is completed
        ///     to calculate elapsed time and update the counter.
        /// </summary>
        /// <param name="name">the name of interval counter.</param>
        /// <returns>callback interface with EndTiming() method </returns>
        public virtual ITiming BeginTiming(string name)
        {
            if (_counters != null)
                return _counters.BeginTiming(name);
            return new EmptyTiming();
        }

        /// <summary>
        ///     Calculates rolling statistics: minimum, maximum, average
        ///     and updates Statistics counter.
        ///     This counter can be used to measure various non-functional
        ///     characteristics, such as amount stored or transmitted data,
        ///     customer feedback, etc.
        /// </summary>
        /// <param name="name">the name of statistics counter.</param>
        /// <param name="value">the value to add to statistics calculations.</param>
        public virtual void Stats(string name, float value)
        {
            if (_counters != null) _counters.Stats(name, value);
        }

        /// <summary>
        ///     Records the last reported value.
        ///     This counter can be used to store performance values reported
        ///     by clients or current numeric characteristics such as number
        ///     of values stored in cache.
        /// </summary>
        /// <param name="name">the name of last value counter</param>
        /// <param name="value">the value to be stored as the last one</param>
        public virtual void Last(string name, float value)
        {
            if (_counters != null) _counters.Last(name, value);
        }

        /// <summary>
        ///     Records the current time.
        ///     This counter can be used to track timing of key business transactions.
        /// </summary>
        /// <param name="name">the name of timing counter</param>
        public virtual void TimestampNow(string name)
        {
            Timestamp(name, DateTime.UtcNow);
        }

        /// <summary>
        ///     Records specified time.
        ///     This counter can be used to tack timing of key
        ///     business transactions as reported by clients.
        /// </summary>
        /// <param name="name">the name of timing counter.</param>
        /// <param name="value">value the reported timing to be recorded.</param>
        public virtual void Timestamp(string name, DateTime value)
        {
            if (_counters != null) _counters.Timestamp(name, value);
        }

        /// <summary>
        ///     Increments counter by value of 1.
        ///     This counter is often used to calculate
        ///     number of client calls or performed transactions.
        /// </summary>
        /// <param name="name">the name of counter counter.</param>
        public virtual void IncrementOne(string name)
        {
            Increment(name, 1);
        }

        /// <summary>
        ///     Increments counter by specified value.
        ///     This counter can be used to track various
        ///     numeric characteristics
        /// </summary>
        /// <param name="name">the name of the increment value.</param>
        /// <param name="value">value number to increase the counter.</param>
        public virtual void Increment(string name, int value)
        {
            if (_counters != null) _counters.Increment(name, value);
        }

        /********* Utility Methods **********/

        /// <summary>
        ///     Generates a string representation for this component
        /// </summary>
        /// <returns>a component descriptor in string format</returns>
        public string toString()
        {
            return _descriptor.ToString();
        }

        /* Performance counters */

        private class EmptyTiming : ITiming
        {
            public void EndTiming()
            {
            }

            public void Dispose()
            {
                EndTiming();
            }
        }
    }
}