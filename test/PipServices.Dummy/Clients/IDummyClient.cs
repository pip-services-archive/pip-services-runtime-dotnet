using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime.Data;

namespace PipServices.Dummy.Clients
{
    public interface IDummyClient
    {
        Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter, PagingParams paging,
            CancellationToken cancellationToken);

        Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId, CancellationToken cancellationToken);
        Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy, CancellationToken cancellationToken);

        Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, DummyObject dummy,
            CancellationToken cancellationToken);

        Task DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken);
    }
}