using Newtonsoft.Json;
using OneIdentity.SafeguardDotNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UiPath.Orchestrator.Extensibility.Configuration;
using UiPath.Orchestrator.Extensibility.SecureStores;

namespace UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard
{
    public class OneIdentitySafeguardSecureStore : ISecureStore
    {
        public SecureStoreInfo GetStoreInfo()
        {
            return new SecureStoreInfo { Identifier = "OneIdentity-Safeguard", IsReadOnly = true };
        }

        public IEnumerable<ConfigurationEntry> GetConfiguration()
        {
            return new List<ConfigurationEntry>
            {
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "Hostname",
                    DisplayName = "Host URL",
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "Thumbprint",
                    DisplayName = "Certificate Thumbprint",
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.Boolean)
                {
                    Key = "IgnoreSSL",
                    DisplayName = "Disable SSL",
                    IsMandatory = true,
                },
            };
        }

        public void Initialize(Dictionary<string, string> hostSettings)
        {
            throw new NotImplementedException();
        }

        public Task ValidateContextAsync(string context)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateCredentialsAsync(string context, string key, Credential value)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateValueAsync(string context, string key, string value)
        {
            throw new NotImplementedException();
        }

        public async Task<Credential> GetCredentialsAsync(string context, string key)
        {
            var client = OneIdentitySafeguardClientFactory.Instance.GetClient(JsonConvert.DeserializeObject<OneIdentitySafeguardContext>(context));
            var password = client.RetrievePassword(key);
            return new Credential { Username = "", Password = password.ToInsecureString() };
        }

        public async Task<string> GetValueAsync(string context, string key)
        {
            return (await GetCredentialsAsync(context, key)).Password;
        }

        public Task RemoveValueAsync(string context, string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateCredentialsAsync(string context, string key, string oldAugumentedKey, Credential value)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateValueAsync(string context, string key, string oldAugumentedKey, string value)
        {
            throw new NotImplementedException();
        }
    }
}
