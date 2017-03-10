using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using Newtonsoft.Json;

namespace PipServices.Runtime.Errors
{
    public sealed class MicroserviceExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var correlationId =
                context.Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "correlation_id").Value;

            var faultData = FaultData.FromException(correlationId, context.Exception);

            var resp = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(faultData)),
                ReasonPhrase = faultData.Category,
                StatusCode = (HttpStatusCode)faultData.Status
            };

            context.Result = new MicroserviceErrorResult(context.Request, resp);
        }
    }
}
