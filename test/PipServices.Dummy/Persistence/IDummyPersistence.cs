using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime;
using PipServices.Runtime.Data;

namespace PipServices.Dummy.Persistence
{
    public interface IDummyPersistence : IPersistence
    {
        Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter, PagingParams paging,
            CancellationToken token);

        Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId, CancellationToken token);
        Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy, CancellationToken token);
        Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy, CancellationToken token);
        Task<DummyObject> DeleteDummyAsync(string correlationId, string dummyId, CancellationToken token);
    }
}