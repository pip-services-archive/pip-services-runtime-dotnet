using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Dummy.Logic;
using PipServices.Runtime;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data;
using PipServices.Runtime.Services;
using PipServices.Runtime.Portability;

namespace PipServices.Dummy.Services
{
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed
        )]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple
        )]
    public class DummyWcfService : WcfService, IDummyWcfService
    {
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Services, "pip-services-dummies", "wcf", "1.0"
            );

        private IDummyBusinessLogic _logic;

        public DummyWcfService()
            : base(ClassDescriptor, typeof(IDummyWcfService))
        {
        }

        public async Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, string key, string skip,
            string take, string total)
        {
            var filter = FilterParams.FromTuples("key", key);
            var paging = new PagingParams(skip, take, total);

            return await _logic.GetDummiesAsync(correlationId, filter, paging, CancellationToken.None);
        }

        public async Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId)
        {
            return await _logic.GetDummyByIdAsync(correlationId, dummyId, CancellationToken.None);
        }

        public async Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy)
        {
            return await _logic.CreateDummyAsync(correlationId, dummy, CancellationToken.None);
        }

        public async Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, DummyObject dummy)
        {
            return await _logic.UpdateDummyAsync(correlationId, dummyId, dummy, CancellationToken.None);
        }

        public async Task DeleteDummyAsync(string correlationId, string dummyId)
        {
            await _logic.DeleteDummyAsync(correlationId, dummyId, CancellationToken.None);
        }

        public override void Link(DynamicMap context, ComponentSet components)
        {
            base.Link(context, components);

            _logic = (IDummyBusinessLogic) components.GetOnePrior(
                this, new ComponentDescriptor(Category.BusinessLogic, "pip-services-dummies", "*", "*")
                );
        }

        public async Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, PartialUpdates dummy)
        {
            return await _logic.UpdateDummyAsync(correlationId, dummyId, dummy, CancellationToken.None);
        }

        public async Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, object dummy)
        {
            return await _logic.UpdateDummyAsync(correlationId, dummyId, dummy, CancellationToken.None);
        }
    }
}