using System;
using System.Net.Http;

namespace ClienteHttp.Abstractions
{
    public interface IHttpRestClientFactory
    {
        HttpClient CreateClient(string name, Action<HttpClient> configureHttpClient = null);
    }
}