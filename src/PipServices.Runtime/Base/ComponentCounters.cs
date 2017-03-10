using System;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Base
{
    /// <summary>
    ///     Component performance counters helper
    /// </summary>
    public class ComponentCounters
    {
        private IComponent _component;
        private readonly ICounters _counters;

        /// <summary>
        ///     Constructs performance counters helper for components.
        /// </summary>
        /// <param name="component">Component reference</param>
        /// <param name="otherComponents">References to other components</param>
        public ComponentCounters(IComponent component, ComponentSet otherComponents)
        {
            _component = component;

            // Get reference to counters component
            _counters = (ICounters) otherComponents.GetOneOptional(
                new ComponentDescriptor(Category.Counters, null, null, null)
                );
        }

        /// <summary>
        ///     Starts measurement of execution time interval.
        ///     The method returns ITiming object that provides endTiming()
        ///     method that shall be called when execution is completed
        ///     to calculate elapsed time and update the counter.
        /// </summary>
        /// <param name="name">the name of interval counter.</param>
        /// <returns>callback interface with EndTiming() method </returns>
        public ITiming BeginTiming(string name)
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
        public void Stats(string name, float value)
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
        public void Last(string name, float value)
        {
            if (_counters != null) _counters.Last(name, value);
        }

        /// <summary>
        ///     Records the current time.
        ///     This counter can be used to track timing of key business transactions.
        /// </summary>
        /// <param name="name">the name of timing counter</param>
        public void TimestampNow(string name)
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
        public void Timestamp(string name, DateTime value)
        {
            if (_counters != null) _counters.Timestamp(name, value);
        }

        /// <summary>
        ///     Increments counter by value of 1.
        ///     This counter is often used to calculate
        ///     number of client calls or performed transactions.
        /// </summary>
        /// <param name="name">the name of counter counter.</param>
        public void IncrementOne(string name)
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
        public void Increment(string name, int value)
        {
            if (_counters != null) _counters.Increment(name, value);
        }

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