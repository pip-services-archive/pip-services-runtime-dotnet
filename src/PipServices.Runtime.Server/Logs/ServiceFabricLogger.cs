using System;
using System.Fabric;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;
using PipServices.Runtime.Run;

namespace PipServices.Runtime.Logs
{
    public sealed class ServiceFabricLogger : AbstractLogger
    {
        /// <summary>
        ///     Unique descriptor for the ServiceFabricLogger component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Logs, "pip-services-runtime-log", "sf-events", "*"
            );

        private readonly MicroserviceEventSource _eventSource = MicroserviceEventSource.Current;
        private object _serviceContext;
        private ServiceFabricMicroserviceType _sfMicroserviceType;

        public ServiceFabricLogger() 
            : base(ClassDescriptor)
        {
        }

        public override void Link(DynamicMap context, ComponentSet components)
        {
            if (_eventSource == null)
                throw new ConfigError("NoEventSource",
                    "Context branch serviceFabric.eventSource must contain event source");

            base.Link(context, components);
        }

        private ServiceFabricMicroserviceType GetAgentType(string type)
        {
            ServiceFabricMicroserviceType result;

            return Enum.TryParse(type, out result) ? result : ServiceFabricMicroserviceType.None;
        }

        /// <summary>
        ///     Writes a message to the log
        /// </summary>
        /// <param name="level">a log level - Fatal, Error, Warn, Info, Debug or Trace</param>
        /// <param name="component">a component name</param>
        /// <param name="correlationId">a correlationId</param>
        /// <param name="message">a message objects</param>
        public override void Log(LogLevel level, string component, string correlationId, object[] message)
        {
            if (_level < level) return;

            var output = LogFormatter.Format(level, message);
            if (correlationId != null)
                output += ", correlated to " + correlationId;

            _serviceContext = Context.Get("service_fabric.service_context");
            _sfMicroserviceType = GetAgentType(Context.GetString("service_fabric.microservice_type"));

            switch (_sfMicroserviceType)
            {
                case ServiceFabricMicroserviceType.StatelessService:
                    _eventSource.Event(level, (StatelessServiceContext)_serviceContext, output);
                    break;
                case ServiceFabricMicroserviceType.StatefulService:
                    _eventSource.Event(level, (StatefulServiceContext)_serviceContext, output);
                    break;
                case ServiceFabricMicroserviceType.Actor:
                    _eventSource.Event(level, (StatefulServiceContext)_serviceContext, output);
                    break;
                default:
                    _eventSource.Message(output);
                    break;
            }
        }
    }
}
