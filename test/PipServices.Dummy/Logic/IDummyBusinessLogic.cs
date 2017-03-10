using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime;
using PipServices.Runtime.Data;

namespace PipServices.Dummy.Logic
{
    public interface IDummyBusinessLogic : IBusinessLogic
    {
        void AddListener(IDummyBusinessLogicListener listener);
        void RemoveListener(IDummyBusinessLogicListener listener);

        Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter, PagingParams paging,
            CancellationToken cancellationToken);

        Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId, CancellationToken cancellationToken);
        Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy, CancellationToken cancellationToken);

        Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy,
            CancellationToken cancellationToken);

        Task<DummyObject> DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken);
    }
}