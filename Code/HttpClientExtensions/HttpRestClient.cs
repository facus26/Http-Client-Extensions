using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Framework.ClienteHttp.Abstractions;
using Framework.ClienteHttp.Exceptions;

namespace Framework.ClienteHttp
{
    public class HttpRestClient : IHttpRestClient
    {
        #region Members
        private readonly HttpClient _client;
        public HttpRequestHeaders DefaultRequestHeaders => _client.DefaultRequestHeaders;
        public Uri BaseAddress
        {
            get => _client.BaseAddress;
            set => _client.BaseAddress = value;
        }
        public long MaxResponseContentBufferSize
        {
            get => _client.MaxResponseContentBufferSize;
            set => _client.MaxResponseContentBufferSize = value;
        }
        public TimeSpan Timeout
        {
            get => _client.Timeout;
            set => _client.Timeout = value;
        }
        #endregion

        #region Constructors
        public HttpRestClient(string name, IHttpRestClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient(name);
        }

        public HttpRestClient(string name, IHttpRestClientFactory clientFactory, Action<HttpClient> configureHttpClient)
        {
            _client = clientFactory.CreateClient(name, configureHttpClient);
        }
        #endregion

        #region Implementation

        #region GET

        public TResponse Get<TResponse>(string method)
        {
            return GetAsync<TResponse>(method).Result;
        }

        public async Task<TResponse> GetAsync<TResponse>(string method)
        {
            var httpResponseMessage = await _client.GetAsync(method).ConfigureAwait(false);

            Validate(httpResponseMessage, method);

            var JSONResult = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResponse>(JSONResult);
        }

        #endregion

        #region POST

        public void Post<TRequest>(string method, TRequest request)
        {
            SendAsync(_client.PostAsync, method, request).Wait();
        }

        public async Task PostAsync<TRequest>(string method, TRequest request)
        {
            await SendAsync(_client.PostAsync, method, request).ConfigureAwait(false);
        }

        public TResponse Post<TResponse, TRequest>(string method, TRequest request)
        {
            return SendAsync<TResponse, TRequest>(_client.PostAsync, method, request).Result;
        }

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string method, TRequest request)
        {
            return await SendAsync<TResponse, TRequest>(_client.PostAsync, method, request).ConfigureAwait(false);
        }

        #endregion

        #region PUT
        public async Task<TResponse> PutAsync<TResponse, TRequest>(string method, TRequest request)
        {
            return await SendAsync<TResponse, TRequest>(_client.PutAsync, method, request).ConfigureAwait(false);
        }

        public TResponse Put<TResponse, TRequest>(string method, TRequest request)
        {
            return SendAsync<TResponse, TRequest>(_client.PutAsync, method, request).Result;
        }

        public async Task PutAsync<TRequest>(string method, TRequest request)
        {
            await SendAsync(_client.PutAsync, method, request).ConfigureAwait(false);
        }

        public void Put<TRequest>(string method, TRequest request)
        {
            SendAsync(_client.PutAsync, method, request).Wait();
        }

        #endregion

        #region Send

        public HttpResponseMessage Send(HttpRequestMessage request)
        {
            return SendAsync(request).Result;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await _client.SendAsync(request).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Método(s) Privado(s)        

        private async Task SendAsync<TRequest>(Func<string, JsonContent, Task<HttpResponseMessage>> methodAsync,
                                                                  string method, TRequest request)
        {
            var httpResponseMessage = await methodAsync(method, new JsonContent(request)).ConfigureAwait(false);
            
            Validate(httpResponseMessage, method);
        }

        private async Task<TResponse> SendAsync<TResponse, TRequest>(Func<string, JsonContent, Task<HttpResponseMessage>> methodAsync,
                                                                                          string path, TRequest request)
        {
            var response = await methodAsync(path, new JsonContent(request)).ConfigureAwait(false);

            Validate(response, path);

            return await ReadAsync<TResponse>(response).ConfigureAwait(false);
        }

        private void Validate(HttpResponseMessage response, string path)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRestClientException(response);
        }

        private async Task<TResponse> ReadAsync<TResponse>(HttpResponseMessage response)
        {
            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResponse>(responseString);
        }

        #endregion
    }
}
