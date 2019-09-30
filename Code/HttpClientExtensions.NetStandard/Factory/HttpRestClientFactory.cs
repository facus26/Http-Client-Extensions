using System;
using System.Net.Http;
using Framework.ClienteHttp.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.ClienteHttp.NetCore.Factory
{
    public class HttpRestClientFactory : IHttpRestClientFactory
    {
        private readonly IServiceProvider _services;

        public HttpRestClientFactory(IServiceProvider services)
        {
            _services = services;
        }

        public HttpClient CreateClient(string name)
        {
            var httpClientFactory = _services.GetService<IHttpClientFactory>();
            return httpClientFactory.CreateClient(name);
        }

        public HttpClient CreateClient(string name, Action<HttpClient> configureHttpClient = null)
        {
            throw new NotImplementedException();
        }
    }
}
