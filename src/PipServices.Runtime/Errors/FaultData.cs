using System;
using System.Runtime.Serialization;

namespace PipServices.Runtime.Errors
{
    [DataContract]
    public class FaultData
    {
        public FaultData()
        {
        }

        public FaultData(string category, IComponent component, string code, string message)
        {
            Category = category ?? ErrorCategory.UnknownError;
            Component = component?.Descriptor.ToString();
            Code = code;
            Message = message ?? "Unknown error";
        }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "component")]
        public string Component { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "status")]
        public int Status { get; set; }

        [DataMember(Name = "correlation_id")]
        public string CorrelationId { get; set; }

        [DataMember(Name = "cause")]
        public Exception Cause { get; set; }

        [DataMember(Name = "details")]
        public object[] Details { get; set; }

        [DataMember(Name = "stack")]
        public string StackTrace { get; set; }

        public static FaultData FromException(string correlationId, Exception ex)
        {
            if (ex == null) return null;

            return new FaultData
            {
                CorrelationId = correlationId,
                Message = (ex as MicroserviceError)?.Message ?? ex.Message,
                StackTrace = ex.StackTrace,
                Cause = ex,
                Status = (ex as MicroserviceError)?.Status ?? 500,
                Category = ex.GetType().Name,
                Component = (ex as MicroserviceError)?.Component ?? string.Empty
            };
        }
    }
}