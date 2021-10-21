using System.Threading.Tasks;
using OneIdentity.SafeguardDotNet.A2A;
using UiPath.Orchestrator.Extensions.SecureStores.Safeguard;
using UiPath.Orchestrator.Extensibility.SecureStores;
using UiPath.Orchestrator.SafeguardSecureStore;

namespace UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard
{
    public class SafeguardClient
    {
        private readonly SafeguardContext _context;
        private ISafeguardA2AContext _vaultClient;

        public SafeguardClient(SafeguardContext context) => _context = context;
        private ISafeguardA2AContext CreateSafeguardConnection() => OneIdentity.SafeguardDotNet.Safeguard.A2A.GetContext(_context.SafeguardAppliance, _context.SafeguardCertThumbprint, apiVersion: 3, ignoreSsl: _context.IgnoreSSL)
                ?? throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardSettingInvalidOrMissing), _context.SafeguardAppliance, _context.SafeguardCertThumbprint, _context.IgnoreSSL));

        public ISafeguardA2AContext GetConnection()
        {
            if (_vaultClient == null)
            {
                _vaultClient = CreateSafeguardConnection();
            }
            return _vaultClient;
        }

        public Task TestConnection()
        {
            GetConnection();
            return Task.CompletedTask;
        }
    }
}