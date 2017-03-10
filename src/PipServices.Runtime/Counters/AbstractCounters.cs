using System;
using System.Collections.Generic;
using System.Threading;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Counters
{
    public abstract class AbstractCounters : AbstractComponent, ICounters
    {
        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "options.timeout", 60000 // timeout in milliseconds
            );

        private readonly Dictionary<string, Counter> _cache = new Dictionary<string, Counter>();
        private Timer _interval;
        private bool _updated;

        protected AbstractCounters(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        public override void Configure(ComponentConfig config)
        {
            base.Configure(config.WithDefaults(DefaultConfig));
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            // Define dump timeout 
            var timeout = Math.Max(1000, _config.Options.GetInteger("timeout"));

            // Stop previously set timer 
            if (_interval != null)
                _interval.Dispose();

            // Set a new timer
            _interval = new Timer(
                state => { Dump(); },
                null, timeout, timeout
                );

            base.Open();
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            // Stop previously set timer
            if (_interval != null)
                _interval.Dispose();

            // Clear timer ID
            _interval = null;

            // Save and clear counters if any
            if (_updated)
            {
                var counters = GetAll();
                Save(counters);
                ResetAll();
            }

            base.Close();
        }

        /// <summary>
        ///     Starts measurement of execution time interval.
        ///     The method returns ITiming object that provides EndTiming()
        ///     method that shall be called when execution is completed
        ///     to calculate elapsed time and update the counter.
        /// </summary>
        /// <param name="name">the name of interval counter.</param>
        /// <returns>
        ///     callback interface with EndTiming() method
        ///     that shall be called at the end of execution.
        /// </returns>
        public override ITiming BeginTiming(string name)
        {
            return new Timing(this, name);
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
        public override void Stats(string name, float value)
        {
            var counter = Get(name, CounterType.Statistics);
            CalculateStats(counter, value);
        }

        /// <summary>
        ///     Records the last reported value.
        ///     This counter can be used to store performance values reported
        ///     by clients or current numeric characteristics such as number
        ///     of values stored in cache.
        /// </summary>
        /// <param name="name">the name of last value counter</param>
        /// <param name="value">the value to be stored as the last one</param>
        public override void Last(string name, float value)
        {
            var counter = Get(name, CounterType.LastValue);
            counter.Last = value;
            _updated = true;
        }

        /// <summary>
        ///     Records specified time.
        ///     This counter can be used to tack timing of key
        ///     business transactions as reported by clients.
        /// </summary>
        /// <param name="name">the name of timing counter.</param>
        /// <param name="value">the reported timing to be recorded.</param>
        public override void Timestamp(string name, DateTime value)
        {
            var counter = Get(name, CounterType.Timestamp);
            counter.Time = value;
            _updated = true;
        }

        /// <summary>
        ///     Increments counter by specified value.
        ///     This counter can be used to track various numeric characteristics
        /// </summary>
        /// <param name="name">the name of counter counter.</param>
        /// <param name="value">number to increase the counter.</param>
        public override void Increment(string name, int value)
        {
            var counter = Get(name, CounterType.Increment);
            counter.Count = counter.Count.HasValue
                ? counter.Count + value
                : value;
            _updated = true;
        }

        protected abstract void Save(List<Counter> counters);

        public void Reset(string name)
        {
            _cache.Remove(name);
        }

        public void ResetAll()
        {
            _cache.Clear();
            _updated = false;
        }

        public void Dump()
        {
            if (_updated)
            {
                var counters = GetAll();
                Save(counters);
            }
        }

        public List<Counter> GetAll()
        {
            return new List<Counter>(_cache.Values);
        }

        public Counter Get(string name, CounterType type)
        {
            if (name == null || name.Length == 0)
                throw new NullReferenceException("Counter name was not set");

            Counter counter = null;
            _cache.TryGetValue(name, out counter);

            if (counter == null || counter.Type != type)
            {
                counter = new Counter(name, type);
                _cache[name] = counter;
            }

            return counter;
        }

        private void CalculateStats(Counter counter, float value)
        {
            if (counter == null)
                throw new NullReferenceException("Missing counter");

            counter.Last = value;
            counter.Count = counter.Count.HasValue ? counter.Count + 1 : 1;
            counter.Max = counter.Max.HasValue ? Math.Max(counter.Max.Value, value) : value;
            counter.Min = counter.Min.HasValue ? Math.Min(counter.Min.Value, value) : value;
            counter.Avg = counter.Avg.HasValue && counter.Count > 1
                ? (counter.Avg*(counter.Count - 1) + value)/counter.Count
                : value;

            _updated = true;
        }

        public void SetTiming(string name, float elapsed)
        {
            var counter = Get(name, CounterType.Interval);
            CalculateStats(counter, elapsed);
        }
    }
}