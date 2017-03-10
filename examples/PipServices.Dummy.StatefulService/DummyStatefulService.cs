using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using PipServices.Dummy.Data;
using PipServices.Dummy.Logic;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Run;

namespace PipServices.Dummy
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class DummyStatefulService : MicroserviceStatefulService, IDummyService
    {
        public DummyStatefulService(StatefulServiceContext serviceContext, Microservice microservice)
            : base(serviceContext, microservice)
        {
        }

        private IDummyBusinessLogic Controller { get; set; }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see http://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new []
            {
                new ServiceReplicaListener(this.CreateServiceRemotingListener)
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);

            Controller = Microservice.GetComponentByCategory(Category.Controllers).FirstOrDefault() as IDummyBusinessLogic;

            long iterations = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                Microservice.Info(this, $"Dummy Statefull Service Working-{++iterations}");

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        public Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter, PagingParams paging,
            CancellationToken cancellationToken)
        {
            return Controller.GetDummiesAsync(correlationId, filter, paging, cancellationToken);
        }

        public Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId, CancellationToken cancellationToken)
        {
            return Controller.GetDummyByIdAsync(correlationId, dummyId, cancellationToken);
        }

        public Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy, CancellationToken cancellationToken)
        {
            return Controller.CreateDummyAsync(correlationId, dummy, cancellationToken);
        }

        public Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy,
            CancellationToken cancellationToken)
        {
            return Controller.UpdateDummyAsync(correlationId, dummyId, dummy, cancellationToken);
        }

        public Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, PartialUpdates dummy,
            CancellationToken cancellationToken)
        {
            return Controller.UpdateDummyAsync(correlationId, dummyId, dummy, cancellationToken);
        }

        public Task DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken)
        {
            return Controller.DeleteDummyAsync(correlationId, dummyId, cancellationToken);
        }
    }
}
