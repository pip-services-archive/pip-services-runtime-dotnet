using System;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Runtime;
using PipServices.Dummy.Run;
using PipServices.Runtime.Run;

namespace PipServices.Dummy
{
    internal static class Program
    {
        private static readonly Microservice Microservice = new DummyMicroservice();

        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                Microservice.LoadConfig("config.withoutservices.yaml");
                Microservice.Start();

                ActorRuntime.RegisterActorAsync<DummyActor>(
                    (context, actorType) =>
                    {
                        var service = new ActorService(context, actorType,
                            (actorService, id) => new DummyActor(actorService, id, Microservice));
                        return service;
                    }).
                    GetAwaiter().GetResult();

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                //Microservice.Error(e.ToString());
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
            finally
            {
                Microservice.Stop();
            }
        }
    }
}
