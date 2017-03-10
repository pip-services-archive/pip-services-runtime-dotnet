using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using PipServices.Dummy.Data;
using PipServices.Dummy.Logic;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Run;

namespace PipServices.Dummy
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class DummyActor : MicroserviceActor, IDummyActor
    {
        private IDummyBusinessLogic Controller { get; set; }

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

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            Microservice.Info(this, "Actor activated.");

            Controller = Microservice.GetComponentByCategory(Category.Controllers).FirstOrDefault() as IDummyBusinessLogic;

            return base.OnActivateAsync();
        }

        public DummyActor(ActorService service, ActorId actorId, Microservice microservice) 
            : base(service, actorId, microservice)
        {
        }
    }
}
