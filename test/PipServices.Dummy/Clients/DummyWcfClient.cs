using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Dummy.Services;
using PipServices.Runtime;
using PipServices.Runtime.Clients;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Portability;

namespace PipServices.Dummy.Clients
{
    public sealed class DummyWcfClient : WcfClient<IDummyWcfService>, IDummyClient
    {
        /// <summary>
        ///     Unique descriptor for the DummyRestClient component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Clients, "pip-services-dummies", "wcf", "1.0"
            );

        public DummyWcfClient()
            : base(ClassDescriptor)
        {
        }

        public DummyWcfClient(ComponentConfig config)
            : this()
        {
            Configure(config);
            Link(new DynamicMap(), new ComponentSet());
        }

        public DummyWcfClient(params object[] values)
            : this()
        {
            var config = new ComponentConfig();
            config.RawContent.SetTuplesArray(values);

            Configure(config);
            Link(new DynamicMap(), new ComponentSet());
        }

        public Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, FilterParams filter,
            PagingParams paging, CancellationToken cancellationToken)
        {
            filter = filter ?? new FilterParams();
            paging = paging ?? new PagingParams();

            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.get_dummies"))
            {
                return Channel.GetDummiesAsync(
                    correlationId,
                    filter.GetNullableString("key"),
                    Converter.ToString(paging.Skip),
                    Converter.ToString(paging.Take),
                    Converter.ToString(paging.Total)
                    );
            }
        }

        public Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.get_dummy_by_id"))
            {
                return Channel.GetDummyByIdAsync(correlationId, dummyId);
            }
        }

        public Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.create_dummy"))
            {
                return Channel.CreateDummyAsync(correlationId, dummy);
            }
        }

        public Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.update_dummy"))
            {
                return Channel.UpdateDummyAsync(correlationId, dummyId, dummy);
            }
        }

        public Task DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.delete_dummy"))
            {
                return Channel.DeleteDummyAsync(correlationId, dummyId);
            }
        }
    }
}