using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime;
using PipServices.Runtime.Clients;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Portability;

namespace PipServices.Dummy.Clients
{
    public sealed class DummyDirectClient : DirectClient, IDummyClient
    {
        /// <summary>
        ///     Unique descriptor for the DummyDirectClient component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Clients, "pip-services-dummies", "direct", "1.0"
            );

        private IBusinessLogic _logic;

        public DummyDirectClient()
            : base(ClassDescriptor)
        {
        }

        public DummyDirectClient(ComponentConfig config)
            : this()
        {
            Configure(config);
            Link(new DynamicMap(), new ComponentSet());
        }

        public DummyDirectClient(params object[] values)
            : this()
        {
            var config = new ComponentConfig();
            config.RawContent.SetTuplesArray(values);

            Configure(config);
            Link(new DynamicMap(), new ComponentSet());
        }

        public async Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter,
            PagingParams paging, CancellationToken cancellationToken)
        {
            return await ExecuteAsync<DataPage<DummyObject>>(
                _logic,
                "get_dummies",
                correlationId,
                DynamicMap.FromTuples(
                    "filter", filter,
                    "paging", paging
                    ),
                cancellationToken
                );
        }

        public async Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            return await ExecuteAsync<DummyObject>(
                _logic,
                "get_dummy_by_id",
                correlationId,
                DynamicMap.FromTuples(
                    "dummy_id", dummyId
                    ),
                cancellationToken
                );
        }

        public async Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            return await ExecuteAsync<DummyObject>(
                _logic,
                "create_dummy",
                correlationId,
                DynamicMap.FromTuples(
                    "dummy", dummy
                    ),
                cancellationToken
                );
        }

        public async Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            return await ExecuteAsync<DummyObject>(
                _logic,
                "update_dummy",
                correlationId,
                DynamicMap.FromTuples(
                    "dummy_id", dummyId,
                    "dummy", dummy
                    ),
                cancellationToken
                );
        }

        public async Task DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken)
        {
            await ExecuteAsync(
                _logic,
                "delete_dummy",
                correlationId,
                DynamicMap.FromTuples(
                    "dummy_id", dummyId
                    ),
                cancellationToken
                );
        }

        public override void Link(DynamicMap context, ComponentSet components)
        {
            base.Link(context, components);

            _logic = (IBusinessLogic) components.GetOneRequired(
                new ComponentDescriptor(Category.BusinessLogic, "pip-services-dummies", "*", "*")
            );
        }
    }
}