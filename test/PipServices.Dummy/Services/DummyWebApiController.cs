using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using PipServices.Dummy.Data;
using PipServices.Dummy.Logic;
using PipServices.Runtime.Data;
using PipServices.Runtime.Services;

namespace PipServices.Dummy.Services
{
    [RoutePrefix("dummies")]
    //[MicroserviceExceptionFilterAttribute]
    public class DummyWebApiController : ApiController, IHttpLogicController<IDummyBusinessLogic>
    {
        public DummyWebApiController()
        {
        }

        public DummyWebApiController(IDummyBusinessLogic logic)
        {
            if (logic == null)
                throw new ArgumentNullException(nameof(logic));

            Logic = logic;
        }

        public IDummyBusinessLogic Logic { get; set; }

        [Route("")]
        [HttpGet]
        public async Task<DataPage<DummyObject>> GetDummiesAsync(
            [FromUri(Name = "correlation_id")] string correlationId = null, string key = null, string skip = null,
            string take = null, string total = null)
        {
            //throw new CallError("call code", "call message");
            //throw new ArgumentNullException(nameof(correlationId));

            var filter = FilterParams.FromTuples("key", key);
            var paging = new PagingParams(skip, take, total);

            return await Logic.GetDummiesAsync(correlationId, filter, paging, CancellationToken.None);
        }

        [Route("{dummyId}")]
        [HttpGet]
        public async Task<DummyObject> GetDummyByIdAsync(string dummyId,
            [FromUri(Name = "correlation_id")] string correlationId = null)
        {
            return await Logic.GetDummyByIdAsync(correlationId, dummyId, CancellationToken.None);
        }

        [Route("")]
        [HttpPost]
        public async Task<DummyObject> CreateDummyAsync([FromBody] DummyObject dummy,
            [FromUri(Name = "correlation_id")] string correlationId = null)
        {
            return await Logic.CreateDummyAsync(correlationId, dummy, CancellationToken.None);
        }

        [Route("{dummyId}")]
        [HttpPut]
        public async Task<DummyObject> UpdateDummyAsync(string dummyId, [FromBody] DummyObject dummy,
            [FromUri(Name = "correlation_id")] string correlationId = null)
        {
            return await Logic.UpdateDummyAsync(correlationId, dummyId, dummy, CancellationToken.None);
        }

        [Route("{dummyId}")]
        [HttpDelete]
        public async Task DeleteDummyAsync(string dummyId,
            [FromUri(Name = "correlation_id")] string correlationId = null)
        {
            await Logic.DeleteDummyAsync(correlationId, dummyId, CancellationToken.None);
        }
    }
}