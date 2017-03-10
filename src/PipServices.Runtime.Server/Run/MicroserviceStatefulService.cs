using System;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;

namespace PipServices.Runtime.Run
{
    public class MicroserviceStatefulService : StatefulService
    {
        protected Microservice Microservice;

        public MicroserviceStatefulService(StatefulServiceContext serviceContext, Microservice microservice)
            : base(serviceContext)
        {
            if (microservice == null)
                throw new ArgumentNullException(nameof(microservice));

            Microservice = microservice;

            Microservice.Context.Set("service_fabric.service_context", serviceContext);
            Microservice.Context.Set("service_fabric.microservice_type", ServiceFabricMicroserviceType.StatefulService);
        }
    }
}
