using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Run
{
    public class MicroserviceActor : Actor
    {
        protected Microservice Microservice { get; set; }

        protected MicroserviceConfig Config { get; set; }

        protected override Task OnActivateAsync()
        {
            Microservice.Context.Set("service_fabric.actor_state_managers." + Id, StateManager);

            return base.OnActivateAsync();
        }

        protected override Task OnDeactivateAsync()
        {
            Microservice.Context.Remove("service_fabric.actor_state_managers." + Id);

            return base.OnDeactivateAsync();
        }

        public MicroserviceActor(ActorService service, ActorId actorId, Microservice microservice)
            : base(service, actorId)
        {
            if (microservice == null)
                throw new ArgumentNullException(nameof(microservice));

            Microservice = microservice;

            Microservice.Context.Set("service_fabric.service_context", service.Context);
            Microservice.Context.Set("service_fabric.microservice_type", ServiceFabricMicroserviceType.Actor);
        }
    }
}