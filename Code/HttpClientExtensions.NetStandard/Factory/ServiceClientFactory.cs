using Framework.ClienteHttp.NetCore.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace Framework.ClienteHttp.NetCore.Factory
{
    public class ServiceClientFactory<TClient> : IServiceClientFactory<TClient>
    {
        private readonly Cache _cache;
        private readonly IServiceProvider _services;

        public ServiceClientFactory(Cache cache, IServiceProvider services)
        {
            _cache = cache;
            _services = services;
        }

        public TClient CreateClient(HttpRestClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return (TClient)_cache.Activator(_services, new object[] { client });
        }

        public class Cache
        {
            private readonly static Func<ObjectFactory> _createActivator = () => ActivatorUtilities.CreateFactory(typeof(TClient), new Type[] { typeof(HttpRestClient), });

            private ObjectFactory _activator;
            private bool _initialized;
            private object _lock;

            public ObjectFactory Activator => LazyInitializer.EnsureInitialized(
                ref _activator,
                ref _initialized,
                ref _lock,
                _createActivator);
        }
    }
}
