using OneIdentity.SafeguardDotNet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using UiPath.Orchestrator.Extensibility.Configuration;
using UiPath.Orchestrator.Extensibility.SecureStores;
using UiPath.Orchestrator.SafeguardSecureStore;

namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public class SafeguardSecureStore : ISecureStore
    {
        public SecureStoreInfo GetStoreInfo() =>
            new SecureStoreInfo { Identifier = "One Identity Safeguard", IsReadOnly = true };


        public void Initialize(Dictionary<string, string> hostSettings)
        {
            //Safeguard does not use host level configuration
        }

        public IEnumerable<ConfigurationEntry> GetConfiguration()
        {
            return new List<ConfigurationEntry>
            {
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "SafeguardAppliance",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingSafeguardAppliance)),
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.Secret)
                {
                    Key = "SafeguardCertThumbprint",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingSafeguardCertificateThumprint)),
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.Boolean)
                {
                    Key = "IgnoreSSL",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingIgnoreSSL)),
                    IsMandatory = true,
                }
            };

        }

        public Task ValidateContextAsync(string context)
        {
            var ctx = ConvertJsonToContext(context);
            SafeguardClientFactory.Instance.GetClient(ctx).TestConnection();
            return Task.CompletedTask;
        }

        // Robots credential APIs
        public async Task<string> GetValueAsync(string context, string key)
        {
            ////key (Robot) should be in format domain\user or machine\user or a2akey:<key> is given in external_name

            var ctx = ConvertJsonToContext(context) ?? throw new Exception();
            var safeguardKey = SafeguardUtils.ExtractKey(key) ?? throw new Exception();
            switch (safeguardKey["SafeguardA2AMethod"])
            {
                case "a2akey":
                    return GetCredential_A2A_APIKey(ctx, new NetworkCredential("", safeguardKey["SafeguardAPIKey"]).SecurePassword).Password ?? throw new Exception();
                case "account_lookup":
                    return GetCredential_A2A_Account(ctx, safeguardKey["SafeguardAccount"], safeguardKey["SafeguardAsset"]).Password ?? throw new Exception();
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

            var ctx = ConvertJsonToContext(context) ?? throw new Exception();
            try
            {
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
                    return GetCredential_A2A_APIKey(ctx, new NetworkCredential("", safeguardKey["SafeguardAPIKey"]).SecurePassword, true);
                case "account_lookup":
                    return new Credential
                    {
                        Username = safeguardKey["SafeguardAccount"],
                        Password = GetCredential_A2A_Account(ctx, safeguardKey["SafeguardAccount"], safeguardKey["SafeguardAsset"]).Password,
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

        public Credential GetCredential_A2A_Account(SafeguardContext context, string targetaccount, string target)
        {

            var a2AContext = SafeguardClientFactory.Instance.GetClient(context).GetConnection();

            List<A2ARetrievableAccount> accounts;
            try
            {
                accounts = (List<A2ARetrievableAccount>)a2AContext.GetRetrievableAccounts();
            } catch (Exception e)
            {
                throw new SafeguardDotNetException("Unable to retrieve accounts from Safeguard. SafeguardAppliance: " + context.SafeguardAppliance + " || SafeguardCertThumprint: ..." + context.SafeguardCertThumbprint.Substring(36) + " || IgnoreSSL: " + context.IgnoreSSL.ToString() + " || Error: " + e.Message);
            }

            var api_key = new SecureString();

            for (int i = 0; i < accounts.Count; i++)
            {
                if (
                    accounts[i].AccountName == targetaccount &&
                    ((accounts[i].AssetName != null && accounts[i].AssetName == target)
                        || (accounts[i].DomainName != null && accounts[i].DomainName == target)
                        || (accounts[i].DomainName != null && accounts[i].DomainName.Split('.')[0] == target)
                        || (accounts[i].AssetNetworkAddress != null && accounts[i].AssetNetworkAddress == target))
                    ) 
                {
                    api_key = accounts[i].ApiKey;
                }
            }

            if (api_key == null )
            {
                throw new SafeguardDotNetException("Cannot find matching retrievable account in Safeguard. Account Name: " + targetaccount + " || Target: " + target + " || Count of retrievable accounts: " + accounts.Count);
            }

            //var retcred = new NetworkCredential("", a2AContext.RetrievePassword(api_key));
            NetworkCredential retcred= new NetworkCredential("", new SecureString());
            try
            {
                retcred.SecurePassword = a2AContext.RetrievePassword(api_key);
            }
            catch (Exception e)
            {
                throw new SafeguardDotNetException("Unable to retrieve password from Safeguard. Error: " + e.Message);
            }

            return new Credential
            {
                Username = targetaccount,
                Password = retcred.Password,
            };
        }

        public Credential GetCredential_A2A_APIKey(SafeguardContext context, SecureString api_key, bool lookup_account = false)
        {
            var a2AContext = SafeguardClientFactory.Instance.GetClient(context).GetConnection();
            if (lookup_account == false)
            {
                var retcred = new NetworkCredential("", a2AContext.RetrievePassword(api_key));
                return new Credential
                {
                    Username = string.Empty,
                    Password = retcred.Password,
                };
            }
            else
            {
                List<OneIdentity.SafeguardDotNet.A2ARetrievableAccount> accounts;
                try
                {
                    accounts = (List<A2ARetrievableAccount>)a2AContext.GetRetrievableAccounts();
                }
                catch (Exception e)
                {
                    throw new SafeguardDotNetException("Unable to retrieve accounts from Safeguard. SafeguardAppliance: " + context.SafeguardAppliance + " || SafeguardCertThumprint: ..." + context.SafeguardCertThumbprint.Substring(36) + " || IgnoreSSL: " + context.IgnoreSSL.ToString() + " || Error: " + e.Message);
                }
                var cred = new Credential();
                try
                {
                    for (int i = 0; i < accounts.Count; i++)
                    {
                        if (new NetworkCredential("", accounts[i].ApiKey).Password == new NetworkCredential("", api_key).Password)
                        {
                            cred.Username = accounts[i].AccountName;
                            cred.Password = new NetworkCredential("", a2AContext.RetrievePassword(api_key)).Password;
                        }
                    }
                } catch (Exception e) 
                {
                    throw new SafeguardDotNetException("Cannot find matching retrievable account in Safeguard, or cannot retrieve password from Safeguard. Username: " + cred.Username + "Error: " + e.Message);
                }
                    
                return cred;
            }
        }
    }
}
