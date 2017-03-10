using System;

namespace PipServices.Runtime.Counters
{
    /// <summary>
    ///     Implementation of ITiming interface that
    ///     provides callback to end measuring execution
    ///     time interface and update interval counter.
    /// </summary>
    public class Timing : ITiming
    {
        private readonly AbstractCounters _counters;
        private readonly string _name;
        private readonly int _start;

        /// <summary>
        ///     Creates instance of timing object that doesn't record anything
        /// </summary>
        public Timing()
        {
        }

        /// <summary>
        ///     Creates instance of timing object that calculates elapsed time
        ///     and stores it to specified performance counters component under specified name.
        /// </summary>
        /// <param name="counters">a performance counters component to store calculated value.</param>
        /// <param name="name">a name of the counter to record elapsed time interval.</param>
        public Timing(AbstractCounters counters, string name)
        {
            _counters = counters;
            _name = name;
            _start = Environment.TickCount;
        }

        /// <summary>
        ///     Completes measuring time interval and updates counter.
        /// </summary>
        public void EndTiming()
        {
            if (_counters != null)
            {
                var elapsed = Environment.TickCount - _start;
                _counters.SetTiming(_name, elapsed);
            }
        }

        /// <summary>
        ///     Callback on object disposal to cleanup memory
        /// </summary>
        public void Dispose()
        {
            EndTiming();
        }
    }
}