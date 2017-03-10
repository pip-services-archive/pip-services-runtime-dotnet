using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PipServices.Runtime.Config;
using PipServices.Runtime.Errors;
using PipServices.Runtime.Portability;

namespace PipServices.Runtime.Clients
{
    public class RestClient : AbstractClient
    {
        private static readonly DynamicMap DefaultConfig = DynamicMap.FromTuples(
            "endpoint.protocol", "http",
            "endpoint.host", "localhost",
            "endpoint.port", "3000"
            );

        private static readonly JsonSerializerSettings TransportSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        public RestClient(ComponentDescriptor descriptor)
            : base(descriptor)
        {
        }

        protected HttpClient Client { get; set; }

        protected string Address { get; set; }

        public override void Configure(ComponentConfig config)
        {
            base.Configure(config.WithDefaults(DefaultConfig));
        }

        public override void Open()
        {
            StartOpening();

            CheckNewStateAllowed(State.Opened);

            var ep = ResolveEndpoint();
            var protocol = ep.Protocol;
            var host = ep.Host;
            var port = ep.Port;

            Address = protocol + "://" + host + ":" + port;

            Client?.Dispose();

            Client = new HttpClient(new HttpClientHandler
            {
                CookieContainer = new CookieContainer(),
                AllowAutoRedirect = true,
                UseCookies = true
            });

            Client.DefaultRequestHeaders.ConnectionClose = true;

            base.Open();
        }

        private static HttpContent CreateEntityContent(object value)
        {
            var content = JsonConvert.SerializeObject(value, TransportSettings);
            var result = new StringContent(content, Encoding.UTF8, "application/json");

            return result;
        }

        private Uri CreateRequestUri(string route)
        {
            var builder = new StringBuilder(Address + "/" + route);

            var result = new Uri(builder.ToString(), UriKind.Absolute);

            return result;
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(HttpMethod method, Uri uri, CancellationToken token,
            HttpContent content = null)
        {
            if (Client == null)
                throw new InvalidOperationException("REST client is not configured");

            HttpResponseMessage result;

            try
            {
                if (method == HttpMethod.Get)
                    result = await Client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, token);
                else if (method == HttpMethod.Post)
                    result = await Client.PostAsync(uri, content, token);
                else if (method == HttpMethod.Put)
                    result = await Client.PutAsync(uri, content, token);
                else if (method == HttpMethod.Delete)
                    result = await Client.DeleteAsync(uri, token);
                else
                    throw new InvalidOperationException("Invalid request type");
            }
            catch (HttpRequestException ex)
            {
                throw new ServerException("Unknown communication problem on REST client", ex);
            }

            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.NoContent:
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.ServiceUnavailable:
                {
                    var responseContent = await result.Content.ReadAsStringAsync();

                    var errorObject = JsonConvert.DeserializeObject<FaultData>(responseContent);

                    var ex = MicroserviceErrorFactory.Create(errorObject);

                    throw ex;
                }
                default:
                {
                    var responseContent = await result.Content.ReadAsStringAsync();

                    throw new ServerException(responseContent);
                }
            }

            return result;
        }

        protected async Task ExecuteAsync(HttpMethod method, string route, CancellationToken token)
        {
            var uri = CreateRequestUri(route);

            using (await ExecuteRequestAsync(method, uri, token))
            {
            }
        }

        protected async Task ExecuteAsync(HttpMethod method, string route, object requestEntity, CancellationToken token)
        {
            var uri = CreateRequestUri(route);

            using (var requestContent = CreateEntityContent(requestEntity))
            {
                using (await ExecuteRequestAsync(method, uri, token, requestContent))
                {
                }
            }
        }

        private static async Task<T> ExtractContentEntityAsync<T>(HttpContent content)
        {
            // ReSharper disable RedundantAssignment

            var result = default(T);

            // ReSharper restore RedundantAssignment

            var value = await content.ReadAsStringAsync();

            try
            {
                result = JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonReaderException ex)
            {
                throw new ServerException("Unexpected protocol format", ex);
            }

            return result;
        }

        protected async Task<T> ExecuteAsync<T>(HttpMethod method, string route, CancellationToken token)
            where T : class
        {
            var uri = CreateRequestUri(route);

            using (var response = await ExecuteRequestAsync(method, uri, token))
            {
                return await ExtractContentEntityAsync<T>(response.Content);
            }
        }

        protected async Task<T> ExecuteAsync<T>(HttpMethod method, string route, object requestEntity,
            CancellationToken token)
            where T : class
        {
            var uri = CreateRequestUri(route);

            using (var requestContent = CreateEntityContent(requestEntity))
            {
                using (var response = await ExecuteRequestAsync(method, uri, token, requestContent))
                {
                    return await ExtractContentEntityAsync<T>(response.Content);
                }
            }
        }

        public override void Close()
        {
            CheckNewStateAllowed(State.Closed);

            Client?.Dispose();

            Client = null;

            base.Close();
        }

        private Endpoint ResolveEndpoint()
        {
            var endpoints = _config.Endpoints;
            if (endpoints.Count == 0)
                throw new ConfigError(this, "NoEndpoint", "Service endpoint is not configured in the client");

            // Todo: Complete implementation
            var endpoint = endpoints[0];

            ValidateEndpoint(endpoint);
            return endpoint;
        }

        private void ValidateEndpoint(Endpoint endpoint)
        {
            // Check for type
            var protocol = endpoint.Protocol;
            if (!"http".Equals(protocol.ToLower()))
                throw new ConfigError(this, "SupportedProtocol", "Protocol type is not supported by REST transport")
                    .WithDetails(protocol);

            // Check for host
            if (endpoint.Host == null)
                throw new ConfigError(this, "NoHost", "No host is configured in REST transport");

            // Check for port
            if (endpoint.Port == 0)
                throw new ConfigError(this, "NoPort", "No port is configured in REST transport");
        }
    }
}