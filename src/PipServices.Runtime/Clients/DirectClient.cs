using System.Threading;
using System.Threading.Tasks;
using PipServices.Runtime.Config;
using PipServices.Runtime.Data.Mapper;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Clients
{
    public class DirectClient : AbstractClient
    {
        protected DirectClient(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        protected virtual async Task<T> ExecuteAsync<T>(IBusinessLogic logic, string command, string correlationId,
            DynamicMap args, CancellationToken cancellationToken)
            where T : class
        {
            var newEntity = await logic.Execute(command, correlationId, args, cancellationToken);

            return ObjectMapper.MapTo<T>(newEntity);
        }

        protected virtual async Task ExecuteAsync(IBusinessLogic logic, string command, string correlationId, DynamicMap args,
            CancellationToken cancellationToken)
        {
            await logic.Execute(command, correlationId, args, cancellationToken);
        }

        //    logic.Execute(command, correlationId, args);
        //{

        //protected virtual void Execute(IBusinessLogic logic, string command, string correlationId, DynamicMap args)
        //}
    }
}