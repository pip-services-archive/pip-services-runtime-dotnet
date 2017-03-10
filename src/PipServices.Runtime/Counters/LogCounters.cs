using System.Collections.Generic;
using PipServices.Runtime.Config;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Counters
{
    public class LogCounters : AbstractCounters
    {
        /// <summary>
        ///     Unique descriptor for the LogCounters component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Counters, "pip-services-runtime-counters", "log", "*"
            );

        /// <summary>
        ///     Creates instance of log counters component.
        /// </summary>
        public LogCounters()
            : base(ClassDescriptor)
        {
        }

        /// <summary>
        ///     Formats counter string representation.
        /// </summary>
        /// <param name="counter">a counter object to generate a string for.</param>
        /// <returns>a formatted string representation of the counter.</returns>
        private string CounterToString(Counter counter)
        {
            var result = "Counter " + counter.Name + " { ";
            result += "\"type\": " + (int) counter.Type;
            if (counter.Last.HasValue)
                result += ", \"last\": " + Converter.ToString(counter.Last.Value);
            if (counter.Count.HasValue)
                result += ", \"count\": " + Converter.ToString(counter.Count.Value);
            if (counter.Min.HasValue)
                result += ", \"min\": " + Converter.ToString(counter.Min.Value);
            if (counter.Max.HasValue)
                result += ", \"max\": " + Converter.ToString(counter.Max.Value);
            if (counter.Avg.HasValue)
                result += ", \"avg\": " + Converter.ToString(counter.Avg.Value);
            if (counter.Time.HasValue)
                result += ", \"time\": " + Converter.ToString(counter.Time.Value);
            result += " }";
            return result;
        }

        /// <summary>
        ///     Outputs a list of counter values to log.
        /// </summary>
        /// <param name="counters">a list of counters to be dump to log.</param>
        protected override void Save(List<Counter> counters)
        {
            if (counters.Count == 0) return;

            counters.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));

            foreach (var counter in counters)
            {
                Debug(null, CounterToString(counter));
            }
        }
    }
}