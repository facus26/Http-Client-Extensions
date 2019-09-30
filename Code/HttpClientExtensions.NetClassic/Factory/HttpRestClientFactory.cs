using Framework.ClienteHttp.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Framework.ClienteHttp.NetClassic.Factory
{
    public class HttpRestClientFactory : IHttpRestClientFactory
    {
        private Dictionary<string, HttpClient> _httpClients;
        public HttpRestClientFactory()
        {
            _httpClients = new Dictionary<string, HttpClient>();
        }        

        public HttpClient CreateClient(string name, Action<HttpClient> configureHttpClient = null)
        {
            if (!_httpClients.TryGetValue(name, out HttpClient httpClient))
            {
                httpClient = new HttpClient();

                configureHttpClient?.Invoke(httpClient);

                _httpClients.Add(name, httpClient);
            }

            return httpClient;
        }        
    }
}
