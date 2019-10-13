using ClienteHttp.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ClienteHttp.Factory
{
    public class HttpRestClientFactory : IHttpRestClientFactory
    {
        private Dictionary<string, HttpClient> _httpClients;
        public HttpRestClientFactory()
        {
            _httpClients = new Dictionary<string, HttpClient>();
        }

        public HttpClient CreateClient(string name)
        {
            if (!_httpClients.TryGetValue(name, out HttpClient httpClient))
            {
                httpClient = new HttpClient();

                _httpClients.Add(name, httpClient);
            }

            return httpClient;
        }

        public HttpClient CreateClient(string name, Action<HttpClient> configureHttpClient = null)
        {
            var httpClient = CreateClient(name);
            configureHttpClient?.Invoke(httpClient);
            return httpClient;
        }
    }
}