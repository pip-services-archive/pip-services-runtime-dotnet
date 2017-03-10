using System;
using System.Runtime.Serialization;

namespace PipServices.Runtime.Counters
{
    [DataContract]
    public class Counter
    {
        public Counter()
        {
        }

        public Counter(string name, CounterType type)
        {
            Name = name;
            Type = type;
        }

        [DataMember(Name = "name", IsRequired = true)]
        public string Name { get; set; }

        [DataMember(Name = "type", IsRequired = true)]
        public CounterType Type { get; set; }

        [DataMember(Name = "last", IsRequired = false)]
        public float? Last { get; set; }

        [DataMember(Name = "count", IsRequired = false)]
        public int? Count { get; set; }

        [DataMember(Name = "min", IsRequired = false)]
        public float? Min { get; set; }

        [DataMember(Name = "max", IsRequired = false)]
        public float? Max { get; set; }

        [DataMember(Name = "avg", IsRequired = false)]
        public float? Avg { get; set; }

        [DataMember(Name = "time", IsRequired = false)]
        public DateTime? Time { get; set; }
    }
}