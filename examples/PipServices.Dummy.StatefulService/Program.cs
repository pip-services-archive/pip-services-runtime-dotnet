using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.ServiceFabric.Services.Runtime;
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

                var name = typeof(DummyStatefulService).Name + "Type";

                ServiceRuntime.RegisterServiceAsync(name, context => new DummyStatefulService(context, Microservice))
                    .GetAwaiter()
                    .GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(DummyStatefulService).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
            finally
            {
                Microservice.Stop();
            }
        }
    }
}
