using System;
using Microsoft.Extensions.Caching.Memory;
using UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard;

namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public sealed class SafeguardClientFactory
    {
        private static readonly Lazy<SafeguardClientFactory> _lazy = new Lazy<SafeguardClientFactory>(() => new SafeguardClientFactory());
        public static SafeguardClientFactory Instance => _lazy.Value;

        private readonly MemoryCache _clients;
        private readonly TimeSpan _vaultClientExpiration = TimeSpan.FromMinutes(10);

        private SafeguardClientFactory()
        {
            var cacheOptions = new MemoryCacheOptions
            {
                SizeLimit = 500,
            };
            _clients = new MemoryCache(cacheOptions);
        }

        public SafeguardClient GetClient(SafeguardContext context)
        {
            return _clients.GetOrCreate(context.GetHashCode(), e =>
            {
                e.Size = 1;
                e.SlidingExpiration = _vaultClientExpiration;
                return new SafeguardClient(context);
            });
        }
    }
}