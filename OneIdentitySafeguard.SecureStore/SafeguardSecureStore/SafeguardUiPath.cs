using OneIdentity.SafeguardDotNet;
using OneIdentity.SafeguardDotNet.A2A;
using System.Collections.Generic;
using System.Security;
using System.Net;
using System.Threading.Tasks;

namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public class SafeguardUiPath
    {
        private static ISafeguardA2AContext _a2AContext;
        private static IList<A2ARetrievableAccount> accounts;
        public string GetCredential_A2A_Account(string appliance, string cert_thumprint, string targetaccount, string target, bool ignoreSsl = true, int apiVersion = 3)
        {

            SecureString api_key = new SecureString();

            _a2AContext = OneIdentity.SafeguardDotNet.Safeguard.A2A.GetContext(appliance, cert_thumprint, apiVersion, ignoreSsl);
            accounts = _a2AContext.GetRetrievableAccounts();

            for (int i = 0; i < accounts.Count; i++)
            {
                if (
                    accounts[i].AccountName == targetaccount &&
                    (accounts[i].AssetName == target 
                        || accounts[i].DomainName == target 
                        || accounts[i].DomainName.Split('.')[0] == target 
                        || accounts[i].AssetNetworkAddress == target)
                    )   
                {
                    api_key = accounts[i].ApiKey;
                }
            }

            return new NetworkCredential("", _a2AContext.RetrievePassword(api_key)).Password;
        }

        public string GetCredential_A2A_APIKey(string appliance, string cert_thumprint, SecureString api_key, bool ignoreSsl = true, int apiVersion = 3)
        {

            _a2AContext = OneIdentity.SafeguardDotNet.Safeguard.A2A.GetContext(appliance, cert_thumprint, apiVersion, ignoreSsl);
            return new NetworkCredential("", _a2AContext.RetrievePassword(api_key)).Password;
        }
    }
}
