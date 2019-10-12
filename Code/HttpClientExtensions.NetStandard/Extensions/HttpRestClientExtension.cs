using ClienteHttp.Abstractions;
using ClienteHttp.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;

namespace ClienteHttp.Extensions
{
    public static class HttpRestClientExtension
    {
        public static IHttpClientBuilder AddHttpRestClient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            return InjectHttpRestClient<TService, TImplementation>(services, typeof(TService).Name);
        }

        public static IHttpClientBuilder AddHttpRestClient<TService, TImplementation>(this IServiceCollection services, string name)
            where TService : class
            where TImplementation : class, TService
        {
            return InjectHttpRestClient<TService, TImplementation>(services, name);
        }

        public static IHttpClientBuilder AddHttpRestClient<TService, TImplementation>(this IServiceCollection services, Action<HttpClient> configureClient)
            where TService : class
            where TImplementation : class, TService
        {
            return InjectHttpRestClient<TService, TImplementation>(services, typeof(TService).Name, configureClient);
        }

        public static IHttpClientBuilder AddHttpRestClient<TService, TImplementation>(this IServiceCollection services, string name, Action<HttpClient> configureClient)
            where TService : class
            where TImplementation : class, TService
        {
            return InjectHttpRestClient<TService, TImplementation>(services, name, configureClient);
        }

        private static IHttpClientBuilder InjectHttpRestClient<TService, TImplementation>(IServiceCollection services, string name, Action<HttpClient> configureClient = null)
            where TService : class
            where TImplementation : class, TService
        {
            services.TryAddSingleton<IHttpRestClientFactory, HttpRestClientFactory>();

            // Service Client
            services.TryAdd(ServiceDescriptor.Transient(typeof(IServiceClientFactory<>), typeof(ServiceClientFactory<>)));
            services.TryAdd(ServiceDescriptor.Transient(typeof(ServiceClientFactory<>.Cache), typeof(ServiceClientFactory<>.Cache)));

            services.AddTransient<TService, TImplementation>(i =>
            {
                var httpClient = new HttpRestClient(name, i.GetService<IHttpRestClientFactory>());
                var serviceClientFactory = i.GetRequiredService<IServiceClientFactory<TImplementation>>();
                return serviceClientFactory.CreateClient(httpClient);
            });

            return configureClient != null ? services.AddHttpClient(name, configureClient) : services.AddHttpClient(name);
        }
    }
}
