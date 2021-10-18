﻿using System;
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
            try
            {
                var ctx = ConvertJsonToContext(context);
                SafeguardClientFactory.Instance.GetClient(ctx).TestConnection();
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardJsonInvalidOrMissing), context));
            }
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
                    return GetCredential_A2A_APIKey(ctx, new NetworkCredential("", safeguardKey["SafeguardAPIKey"]).SecurePassword).Password;
                case "account_lookup":
                    return GetCredential_A2A_Account(ctx, safeguardKey["SafeguardAccount"], safeguardKey["SafeguardAsset"]).Password;
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

            var ctx = ConvertJsonToContext(context);
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
            var accounts = a2AContext.GetRetrievableAccounts();
            var api_key = new SecureString();

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

            var retcred = new NetworkCredential("", a2AContext.RetrievePassword(api_key));

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
                var accounts = a2AContext.GetRetrievableAccounts();
                var cred = new Credential();
                for (int i = 0; i < accounts.Count; i++)
                {
                    if (new NetworkCredential("", accounts[i].ApiKey).Password == new NetworkCredential("", api_key).Password)
                    {
                        cred.Username = accounts[i].AccountName;
                        cred.Password = new NetworkCredential("", a2AContext.RetrievePassword(api_key)).Password;
                    }
                }
                return cred;
            }
        }
    }
}
