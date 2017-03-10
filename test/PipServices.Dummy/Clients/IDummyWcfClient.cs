using System.ServiceModel;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using PipServices.Dummy.Data;
using PipServices.Runtime.Data;
using PipServices.Runtime.Errors;

namespace PipServices.Dummy.Clients
{
    [ServiceKnownType(typeof(DummyObject))]
    [ServiceContract(Namespace = "http://PipServices/Dummy", Name = "IDummyWcfService")]
    public interface IDummyWcfClient
    {
        [OperationContract]
        [WebGet(
            UriTemplate = "dummies?correlation_id={correlationId}&key={key}&skip={skip}&take={take}&total={total}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json
            )]
        [FaultContract(typeof(FaultData))]
        Task<DataPage<DummyObject>> GetDummiesAsync(string correlationId, string key, string skip, string take,
            string total);

        [OperationContract]
        [WebGet(
            UriTemplate = "dummies/{dummyId}?correlation_id={correlationId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json
            )]
        [FaultContract(typeof(FaultData))]
        Task<DummyObject> GetDummyByIdAsync(string correlationId, string dummyId);

        [OperationContract]
        [WebInvoke(
            Method = "POST",
            UriTemplate = "dummies?correlation_id={correlationId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json
            )]
        [FaultContract(typeof(FaultData))]
        Task<DummyObject> CreateDummyAsync(string correlationId, DummyObject dummy);

        [OperationContract]
        [WebInvoke(
            Method = "PUT",
            UriTemplate = "dummies/{dummyId}?correlation_id={correlationId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json
            )]
        [FaultContract(typeof(FaultData))]
        Task<DummyObject> UpdateDummyAsync(string correlationId, string dummyId, DummyObject dummy);

        [OperationContract]
        [WebInvoke(
            Method = "DELETE",
            UriTemplate = "dummies/{dummyId}?correlation_id={correlationId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json
            )]
        [FaultContract(typeof(FaultData))]
        Task DeleteDummyAsync(string correlationId, string dummyId);
    }
}