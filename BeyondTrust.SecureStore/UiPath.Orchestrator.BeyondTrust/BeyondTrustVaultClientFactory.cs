using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UiPath.Orchestrator.BeyondTrust
{
    public sealed class BeyondTrustVaultClientFactory
    {
        private static readonly Lazy<BeyondTrustVaultClientFactory> _lazy = new Lazy<BeyondTrustVaultClientFactory>(() => new BeyondTrustVaultClientFactory());
        public static BeyondTrustVaultClientFactory Instance => _lazy.Value;

        private readonly MemoryCache _clients;
        private readonly TimeSpan _vaultClientExpiration = TimeSpan.FromMinutes(10);
        private BeyondTrustVaultClientFactory()
        {
            var cacheOptions = new MemoryCacheOptions
            {
                SizeLimit = 500,
            };
            _clients = new MemoryCache(cacheOptions);
        }

        public BeyondTrustVaultClient GetClient(string context)
        {
            return _clients.GetOrCreate(context.GetHashCode(), e =>
            {
                e.Size = 1;
                e.SlidingExpiration = _vaultClientExpiration;
                var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(context);
                return new BeyondTrustVaultClient(config);
            });
        }
    }
}
