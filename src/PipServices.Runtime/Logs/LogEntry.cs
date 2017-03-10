using System;
using System.Runtime.Serialization;

namespace PipServices.Runtime.Logs
{
    [DataContract]
    public class LogEntry
    {
        public LogEntry()
        {
        }

        public LogEntry(LogLevel level, string component, string correlationId, params object[] message)
        {
            Time = DateTime.UtcNow;
            Component = component;
            Level = level;
            CorrelationId = correlationId;
            Message = message;
        }

        [DataMember(Name = "time", IsRequired = true)]
        public DateTime Time { get; set; }

        [DataMember(Name = "component", IsRequired = false)]
        public string Component { get; set; }

        [DataMember(Name = "level", IsRequired = true)]
        public LogLevel Level { get; set; }

        [DataMember(Name = "correlationId", IsRequired = false)]
        public string CorrelationId { get; set; }

        [DataMember(Name = "message", IsRequired = false)]
        public object[] Message { get; set; }
    }
}