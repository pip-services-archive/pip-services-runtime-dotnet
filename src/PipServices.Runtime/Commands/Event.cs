using System;
using System.Collections.Generic;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Events to receive notifications on command execution results and failures.
    /// </summary>
    public class Event : IEvent
    {
        private readonly IComponent _component;

        /// <summary>
        ///     Creates and initializes instance of the event
        /// </summary>
        /// <param name="component">Component reference</param>
        /// <param name="name">Name of the event</param>
        public Event(IComponent component, string name)
        {
            if (name == null)
                throw new NullReferenceException("Event name is not set");

            _component = component;
            Name = name;
        }

        /// <summary>
        ///     The event name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Listeners that receive notifications for that event
        /// </summary>
        public List<IEventListener> Listeners { get; } = new List<IEventListener>();

        /// <summary>
        ///     Adds listener to receive notifications
        /// </summary>
        /// <param name="listener">Listener reference to be added</param>
        public void AddListener(IEventListener listener)
        {
            Listeners.Add(listener);
        }

        /// <summary>
        ///     Removed listener for event notifications.
        /// </summary>
        /// <param name="listener">Listener refernece to be removed</param>
        public void RemoveListener(IEventListener listener)
        {
            Listeners.Remove(listener);
        }

        /// <summary>
        ///     Notifies all listeners about the event.
        /// </summary>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="value">Event value</param>
        public void Notify(string correlationId, DynamicMap value)
        {
            foreach (var listener in Listeners)
            {
                try
                {
                    listener.OnEvent(this, correlationId, value);
                }
                catch (Exception ex)
                {
                    // Wrap the error
                    var error = new UnknownError(
                        _component,
                        "EventFailed",
                        "Rasing event " + Name + " failed: " + ex
                        )
                        .WithDetails(Name)
                        .WithCorrelationId(correlationId)
                        .Wrap(ex);

                    // Output the error
                    var component = _component as AbstractComponent;
                    if (component != null)
                    {
                        component.Error(correlationId, error);
                    }
                }
            }
        }
    }
}