using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace PipServices.Runtime.Errors
{
    public sealed class MicroserviceExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
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

            context.Response = resp;
        }
    }
}
