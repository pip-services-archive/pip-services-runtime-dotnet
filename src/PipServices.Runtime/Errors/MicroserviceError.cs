using System;
using System.Net;
using System.ServiceModel.Web;

namespace PipServices.Runtime.Errors
{
    [Serializable]
    public class MicroserviceError : WebFaultException<FaultData>
    {
        public MicroserviceError(string category, int status, IComponent component, string code, string message)
            : base(
                new FaultData(category ?? ErrorCategory.UnknownError, component, code ?? "Undefined",
                    message ?? "Unknown error"),
                (HttpStatusCode) status
                )
        {
            Detail.Status = status;
            Detail.StackTrace = Environment.StackTrace;
        }

        public string Category
        {
            get { return Detail.Category; }
            internal set { Detail.Category = value; }
        }

        public string Component
        {
            get { return Detail.Component; }
            internal set { Detail.Component = value; }
        }

        public new string Message
        {
            get { return Detail.Message; }
        }

        public new string Code
        {
            get { return Detail.Code; }
            internal set { Detail.Code = value; }
        }

        public int Status
        {
            get { return Detail.Status; }
        }

        public object[] Details
        {
            get { return Detail.Details; }
            internal set { Detail.Details = value; }
        }

        public string CorrelationId
        {
            get { return Detail.CorrelationId; }
            internal set { Detail.CorrelationId = value; }
        }

        public Exception Cause
        {
            get { return Detail.Cause; }
            internal set
            {
                Detail.Cause = value;
                Detail.StackTrace = value.StackTrace;
            }
        }

        public MicroserviceError ForComponent(string component)
        {
            Component = component;
            return this;
        }

        public MicroserviceError WithCode(string code)
        {
            Code = code ?? "UnknownError";
            return this;
        }

        //public MicroserviceError WithStatus(int status)
        //{
        //    Status = status;
        //    return this;
        //}

        public MicroserviceError WithDetails(params object[] details)
        {
            Details = details;
            return this;
        }

        public MicroserviceError WithCause(Exception cause)
        {
            Cause = cause;
            return this;
        }

        public MicroserviceError WithCorrelationId(string correlationId)
        {
            CorrelationId = correlationId;
            return this;
        }

        public MicroserviceError Wrap(Exception cause)
        {
            if (cause is MicroserviceError)
                return (MicroserviceError) cause;

            WithCause(cause);
            return this;
        }

        public static MicroserviceError Wrap(MicroserviceError error, Exception cause)
        {
            if (cause is MicroserviceError)
                return (MicroserviceError) cause;

            error.WithCause(cause);
            return error;
        }
    }
}