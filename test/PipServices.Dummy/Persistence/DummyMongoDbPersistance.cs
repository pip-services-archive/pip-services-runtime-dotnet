using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Persistence;

namespace PipServices.Dummy.Persistence
{
    public sealed class DummyMongoDbPersistance : MongoDbPersistence<DummyObject>, IDummyPersistence
    {
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Persistence, "pip-services-dummies", "mongodb", "*"
            );

        public DummyMongoDbPersistance()
            : base("dummies", ClassDescriptor)
        {
        }

        public DummyMongoDbPersistance(ComponentDescriptor descriptor)
            : base("dummies", descriptor)
        {
        }

        public async Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter,
            PagingParams paging, CancellationToken cancellationToken)
        {
            return await GetPageAsync<DummyTerms>(correlationId, filter, paging, null, cancellationToken);
        }

        public async Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            return await GetByIdAsync(correlationId, dummyId, cancellationToken);
        }

        public async Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            return await CreateAsync(correlationId, dummy, cancellationToken);
        }

        public async Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy,
            CancellationToken cancellationToken)
        {
            return await UpdateAsync(correlationId, dummyId, dummy, cancellationToken);
        }

        public async Task<DummyObject> DeleteDummyAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            return await DeleteAsync(correlationId, dummyId, cancellationToken);
        }
    }
}