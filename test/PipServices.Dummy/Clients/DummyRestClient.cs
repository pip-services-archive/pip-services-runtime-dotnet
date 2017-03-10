using System.Net.Http;
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
    public sealed class DummyRestClient : RestClient, IDummyClient
    {
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Clients, "pip-services-dummies", "rest", "1.0"
            );

        public DummyRestClient()
            : base(ClassDescriptor)
        {
        }

        public DummyRestClient(ComponentConfig config)
            : this()
        {
            Configure(config);
            Link(new DynamicMap(), new ComponentSet());
        }

        public DummyRestClient(params object[] values)
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
                return ExecuteAsync<DataPage<DummyObject>>(
                    HttpMethod.Get,
                    $"dummies?correlation_id={correlationId}&key={filter.GetNullableString("key")}&skip={Converter.ToString(paging.Skip)}&take={Converter.ToString(paging.Take)}&total={Converter.ToString(paging.Total)}",
                    cancellationToken
                    );
            }
        }

        public Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId,
            CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.get_dummy_by_id"))
            {
                return ExecuteAsync<DummyObject>(
                    HttpMethod.Get,
                    $"dummies/{dummyId}?correlation_id={correlationId}",
                    cancellationToken
                    );
            }
        }

        public Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.create_dummy"))
            {
                return ExecuteAsync<DummyObject>(
                    HttpMethod.Post,
                    $"dummies?correlation_id={correlationId}",
                    dummy,
                    cancellationToken
                    );
            }
        }

        public Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, DummyObject dummy,
            CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.update_dummy"))
            {
                return ExecuteAsync<DummyObject>(
                    HttpMethod.Put,
                    $"dummies/{dummyId}?correlation_id={correlationId}",
                    dummy,
                    cancellationToken
                    );
            }
        }

        public Task DeleteDummyAsync(string correlationId, string dummyId, CancellationToken cancellationToken)
        {
            CheckCurrentState(State.Opened);

            using (var timing = Instrument(correlationId, "dummy.delete_dummy"))
            {
                return ExecuteAsync(
                    HttpMethod.Delete,
                    $"dummies/{dummyId}?correlation_id={correlationId}",
                    cancellationToken
                    );
            }
        }
    }
}