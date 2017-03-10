using System;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;

namespace PipServices.Runtime.Run
{
    public class MicroserviceStatelessService : StatelessService
    {
        protected Microservice Microservice;

        public MicroserviceStatelessService(StatelessServiceContext serviceContext, Microservice microservice)
            : base(serviceContext)
        {
            if (microservice == null)
                throw new ArgumentNullException(nameof(microservice));

            Microservice = microservice;

            Microservice.Context.Set("service_fabric.service_context", serviceContext);
            Microservice.Context.Set("service_fabric.microservice_type", ServiceFabricMicroserviceType.StatelessService);
        }
    }
}