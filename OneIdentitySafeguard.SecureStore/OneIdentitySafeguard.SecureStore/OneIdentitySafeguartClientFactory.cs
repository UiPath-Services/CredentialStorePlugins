using System;
using Microsoft.Extensions.Caching.Memory;

namespace UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard
{
    public sealed class OneIdentitySafeguardClientFactory
    {
        private static readonly Lazy<OneIdentitySafeguardClientFactory> _lazy = new Lazy<OneIdentitySafeguardClientFactory>(() => new OneIdentitySafeguardClientFactory());
        public static OneIdentitySafeguardClientFactory Instance => _lazy.Value;

        private readonly MemoryCache _clients;
        private readonly TimeSpan _vaultClientExpiration = TimeSpan.FromMinutes(10);

        private OneIdentitySafeguardClientFactory()
        {
            var cacheOptions = new MemoryCacheOptions
            {
                SizeLimit = 500,
            };
            _clients = new MemoryCache(cacheOptions);
        }

        public OneIdentitySafeguardClient GetClient(OneIdentitySafeguardContext context)
        {
            return _clients.GetOrCreate(context.GetHashCode(), e =>
            {
                e.Size = 1;
                e.SlidingExpiration = _vaultClientExpiration;
                return new OneIdentitySafeguardClient(context);
            });
        }
    }
}
