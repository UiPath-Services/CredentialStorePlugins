using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using UiPath.Orchestrator.Extensibility.SecureStores;
using UiPath.Orchestrator.Extensibility.Configuration;
using System.Security;
using OneIdentity.SafeguardDotNet.A2A;
using OneIdentity.SafeguardDotNet;
using System.Net;
using UiPath.Orchestrator.SafeguardSecureStore;

namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public class SafeguardSecureStore : ISecureStore
    {
        private const string NameIdentifier = "Safeguard";

        

        public SecureStoreInfo GetStoreInfo() =>
            new SecureStoreInfo { Identifier = NameIdentifier, IsReadOnly = true };

        // Configuration APIs
        public void Initialize(Dictionary<string, string> hostSettings)
        {
            //Safeguard does not use host level configuration
            //https://github.com/UiPath/Orchestrator-CredentialStorePlugins#initialization-and-configuration
        }

        public IEnumerable<ConfigurationEntry> GetConfiguration()
        {
            return new List<ConfigurationEntry>
            {
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "SafeguardAppliance",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingSafeguardAppliance)),                    
                    IsMandatory = true
                },
                new ConfigurationValue(ConfigurationValueType.Secret)
                {
                    Key = "SafeguardCertThumbprint",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingSafeguardCertificateThumprint)),
                    IsMandatory = true
                }
            };

        }

        public Task ValidateContextAsync(string context)
        {
            //The parameter context from all methods on the interface is a json-serialized representation of the instance-level configuration that is defined by the method GetConfiguration.
            //ImplementConfigValidationMethod(context);

            try
            {
                var ctx = ConvertJsonToContext(context);

                if (!string.IsNullOrWhiteSpace(ctx.SafeguardAppliance) &&
                    !string.IsNullOrWhiteSpace(ctx.SafeguardCertThumbprint))
                {
                    return Task.CompletedTask;
                }
            }
            catch (Exception)
            {
                throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardJsonInvalidOrMissing), context));
            }

            return Task.CompletedTask;
        }

        // Robots credential APIs
        public async Task<string> GetValueAsync(string context, string key)
        {
            ////key (Robot) should be in format domain\user or machine\user or a2akey:<key> is given in external_name

            var ctx = ConvertJsonToContext(context);
            var safeguardKey = SafeguardUtils.ExtractKey(key);
            switch (safeguardKey["SafeguardA2AMethod"])
            {
                case "a2akey":
                    return GetCredential_A2A_APIKey(ctx.SafeguardAppliance, ctx.SafeguardCertThumbprint, new NetworkCredential("", safeguardKey["SafeguardAPIKey"]).SecurePassword).Password;
                case "account_lookup":
                    return GetCredential_A2A_Account(ctx.SafeguardAppliance, ctx.SafeguardCertThumbprint, safeguardKey["SafeguardAccount"], safeguardKey["SafeguardAsset"]).Password;
                default:
                    throw new SecureStoreException(
                        SecureStoreException.Type.SecretNotFound,
                        "Unknown SafeguardA2AMethod");
            }
            
        }

        public Task<string> CreateValueAsync(string context, string key, string value) =>
            throw new SecureStoreException(
                SecureStoreException.Type.UnsupportedOperation,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardReadOnly)));
        
        

        public Task<string> UpdateValueAsync(string context, string key, string oldAugumentedKey, string value) =>
            throw new SecureStoreException(
                SecureStoreException.Type.UnsupportedOperation,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardReadOnly)));


        // Assets credential APIs
        public async Task<Credential> GetCredentialsAsync(string context, string key)
        {
            //key (Asset Name) should be in format user@target or a2akey:<key> is given in external_name

            var ctx = new SafeguardContext();

            try
            {
                ctx = ConvertJsonToContext(context);

                if (string.IsNullOrWhiteSpace(ctx.SafeguardAppliance) ||
                    string.IsNullOrWhiteSpace(ctx.SafeguardCertThumbprint))
                {
                    throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardJsonInvalidOrMissing), context));
                }
            }
            catch (Exception)
            {
                throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardJsonInvalidOrMissing), context));
            }

            var safeguardKey = SafeguardUtils.ExtractKey(key);
            
            switch (safeguardKey["SafeguardA2AMethod"])
            {
                case "a2akey":
                    return GetCredential_A2A_APIKey(ctx.SafeguardAppliance, ctx.SafeguardCertThumbprint, new NetworkCredential("", safeguardKey["SafeguardAPIKey"]).SecurePassword, true);
                case "account_lookup":
                    return new Credential
                    {
                        Username = safeguardKey["SafeguardAccount"],
                        Password = GetCredential_A2A_Account(ctx.SafeguardAppliance, ctx.SafeguardCertThumbprint, safeguardKey["SafeguardAccount"], safeguardKey["SafeguardAsset"]).Password,
                    };
                default:
                    throw new SecureStoreException(
                        SecureStoreException.Type.SecretNotFound,
                        "Unknown SafeguardA2AMethod");
            }
        }

        public Task<string> CreateCredentialsAsync(string context, string key, Credential value) =>
            throw new SecureStoreException(
                SecureStoreException.Type.UnsupportedOperation,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardReadOnly)));

        public Task<string> UpdateCredentialsAsync(string context, string key, string oldAugumentedKey, Credential value) =>
            throw new SecureStoreException(
                SecureStoreException.Type.UnsupportedOperation,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardReadOnly)));

        // deletion for both Asstes and Robots credentials
        public Task RemoveValueAsync(string context, string key) =>
            throw new SecureStoreException(
                SecureStoreException.Type.UnsupportedOperation,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardReadOnly)));

        protected static SafeguardContext ConvertJsonToContext(string context)
        {
            return new SafeguardContextBuilder().FromJson(context).Build();

        }

        //move A2A get context to Client.cs file
        private static ISafeguardA2AContext _a2AContext;
        private static IList<A2ARetrievableAccount> accounts;
        public Credential GetCredential_A2A_Account(string appliance, string cert_thumprint, string targetaccount, string target, bool ignoreSsl = true, int apiVersion = 3)
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

            NetworkCredential retcred = new NetworkCredential("", _a2AContext.RetrievePassword(api_key));

            _a2AContext.Dispose();

            return new Credential
            {
                Username = targetaccount,
                Password = retcred.Password,
            };
        }

        public Credential GetCredential_A2A_APIKey(string appliance, string cert_thumprint, SecureString api_key, bool lookup_account = false, bool ignoreSsl = true, int apiVersion = 3)
        {

            _a2AContext = OneIdentity.SafeguardDotNet.Safeguard.A2A.GetContext(appliance, cert_thumprint, apiVersion, ignoreSsl);
            if (lookup_account == false)
            {
                NetworkCredential retcred = new NetworkCredential("", _a2AContext.RetrievePassword(api_key));
                _a2AContext.Dispose();
                return new Credential
                {
                    Username = string.Empty,
                    Password = retcred.Password,
                };
            }
            else
            {
                accounts = _a2AContext.GetRetrievableAccounts();
                Credential cred = new Credential();
                for (int i = 0; i < accounts.Count; i++)
                {
                    if (new NetworkCredential("", accounts[i].ApiKey).Password == new NetworkCredential("",api_key).Password)
                    {
                        cred.Username = accounts[i].AccountName;
                        cred.Password = new NetworkCredential("", _a2AContext.RetrievePassword(api_key)).Password;
                    }
                }
                _a2AContext.Dispose();
                return cred;
            }
        }
    }
}
