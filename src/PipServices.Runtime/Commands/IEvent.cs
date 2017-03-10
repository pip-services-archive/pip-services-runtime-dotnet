using System.Collections.Generic;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Commands
{
    /// <summary>
    ///     Interface for command events.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        ///     The event name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Listeners that receive notifications for that event
        /// </summary>
        List<IEventListener> Listeners { get; }

        /// <summary>
        ///     Adds listener to receive notifications
        /// </summary>
        /// <param name="listener">Listener reference to be added</param>
        void AddListener(IEventListener listener);

        /// <summary>
        ///     Removed listener for event notifications.
        /// </summary>
        /// <param name="listener">Listener refernece to be removed</param>
        void RemoveListener(IEventListener listener);

        /// <summary>
        ///     Notifies all listeners about the event.
        /// </summary>
        /// <param name="correlationId">a unique correlation/transaction id</param>
        /// <param name="value">Event value</param>
        void Notify(string correlationId, DynamicMap args);
    }
}