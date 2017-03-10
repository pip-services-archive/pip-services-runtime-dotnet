using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using PipServices.Dummy.Data;
using PipServices.Runtime.Data;

namespace PipServices.Dummy
{
    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDummyActor : IActor
    {
        Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter, PagingParams paging,
            CancellationToken cancellationToken);

        Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId, CancellationToken cancellationToken);
        Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy, CancellationToken cancellationToken);

        Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy,
            CancellationToken cancellationToken);

        Task DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken);
    }
}
