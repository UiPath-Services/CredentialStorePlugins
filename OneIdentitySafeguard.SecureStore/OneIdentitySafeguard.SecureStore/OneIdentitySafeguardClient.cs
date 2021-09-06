using OneIdentity.SafeguardDotNet;
using OneIdentity.SafeguardDotNet.A2A;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard
{
    public class OneIdentitySafeguardClient
    {
        private readonly OneIdentitySafeguardContext _context;
        private ISafeguardA2AContext _vaultClient;

        public OneIdentitySafeguardClient(OneIdentitySafeguardContext context) => _context = context;
        private ISafeguardA2AContext CreateVaultClient() => Safeguard.A2A.GetContext(_context.Hostname, _context.Thumbprint, ignoreSsl: _context.IgnoreSSL);

        private ISafeguardA2AContext GetConnection()
        {
            if (_vaultClient == null)
            {
                _vaultClient = CreateVaultClient();
            }
            return _vaultClient;
        }

        public SecureString RetrievePassword(string apiKey)
        {
            var secureKey = new NetworkCredential("", apiKey).SecurePassword;
            return GetConnection().RetrievePassword(secureKey);
        }

        public Task TestConnection()
        {
            GetConnection();
            return Task.CompletedTask;
        }
    }
}