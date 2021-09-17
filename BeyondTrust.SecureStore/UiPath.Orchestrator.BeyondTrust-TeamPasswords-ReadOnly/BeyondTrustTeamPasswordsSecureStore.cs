﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UiPath.Orchestrator.BeyondTrust;
using UiPath.Orchestrator.Extensibility.Configuration;
using UiPath.Orchestrator.Extensibility.SecureStores;

namespace UiPath.Orchestrator.BeyondTrustTeamPasswordsReadOnly
{
    public class BeyondTrustTeamPasswordsSecureStore : ISecureStore
    {
        public void Initialize(Dictionary<string, string> hostSettings)
        {
            // No host level settings
        }

        public Task ValidateContextAsync(string context)
        {
            BeyondTrustVaultClientFactory.GetClient(context).TestConnection();
            return Task.CompletedTask;
        }

        public IEnumerable<ConfigurationEntry> GetConfiguration()
        {
            return new List<ConfigurationEntry>
            {
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "Hostname",
                    DisplayName = "BeyondTrust Host URL",
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "AuthKey",
                    DisplayName = "API Authentication Key",
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "RunAs",
                    DisplayName = "API Run As",
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.Boolean)
                {
                    Key = "SSLEnabled",
                    DisplayName = "Use SSL certificate",
                    IsMandatory = true,
                },
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "FolderPasswordDelimiter",
                    DisplayName = "Folder / Account delimiter",
                    IsMandatory = true,
                },
            };
        }

        public SecureStoreInfo GetStoreInfo()
        {
            return new SecureStoreInfo { Identifier = "BeyondTrust-TeamPasswords-ReadOnly", IsReadOnly = true };
        }

        public async Task<string> CreateCredentialsAsync(string context, string key, Credential value)
        {
            throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "This secure store is read-only");
        }

        public async Task<Credential> GetCredentialsAsync(string context, string key)
        {
            var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(context);
            var splitDelimiter = new string[] { config["FolderPasswordDelimiter"].ToString() };
            var keyPieces = key.Split(splitDelimiter, StringSplitOptions.None);

            if (keyPieces.Length > 2)
            {
                throw new SecureStoreException(SecureStoreException.Type.InvalidConfiguration, "Splitting by delimiter " + splitDelimiter + " results in " + keyPieces.Length.ToString() + " parts");
            }

            var teamPasswordFolderName = keyPieces[0];
            var teamPasswordName = keyPieces[1];

            var client = BeyondTrustVaultClientFactory.GetClient(context);
            client.SignIn();
            var teamPasswordFoldersResult = client.TeamPasswordsFolders.GetAll();
            if (!teamPasswordFoldersResult.IsSuccess)
            {
                client.SignOut();
                if (teamPasswordFoldersResult.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "Team Folder not found");
                }
                else
                {
                    throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "Team Folder retreival failed");
                }
            }

            var teamPasswordFolder = teamPasswordFoldersResult.Value.Find(f => teamPasswordFolderName.Equals(f.Name));

            var teamPasswordsIncompleteResult = client.TeamPasswordsCredentials.GetAll(teamPasswordFolder.Id);
            if (!teamPasswordsIncompleteResult.IsSuccess)
            {
                client.SignOut();
                if (teamPasswordsIncompleteResult.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "Team Password not found");
                }
                else
                {
                    throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "Team Password retreival failed");
                }
            }

            // Doesn't contain password
            var teamPasswordIncomplete = teamPasswordsIncompleteResult.Value.Find(p => teamPasswordName.Equals(p.Title));

            var teamPasswordResult = client.TeamPasswordsCredentials.Get(teamPasswordIncomplete.Id);
            if (!teamPasswordResult.IsSuccess)
            {
                client.SignOut();
                if (teamPasswordResult.StatusCode.Equals(HttpStatusCode.NotFound))
                {
                    throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "Team Password not found");
                }
                else
                {
                    throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "Team Password retreival failed");
                }
            }

            client.SignOut();

            return new Credential { Username = teamPasswordResult.Value.Username, Password = teamPasswordResult.Value.Password };


        }

        public async Task<string> UpdateCredentialsAsync(string context, string key, string oldAugumentedKey, Credential value)
        {
            throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "This secure store is read-only");
        }

        public async Task<string> CreateValueAsync(string context, string key, string value)
        {
            throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "This secure store is read-only");
        }

        public async Task<string> GetValueAsync(string context, string key)
        {
            Credential credential = await GetCredentialsAsync(context, key);
            return credential.Password;
        }

        public Task<string> UpdateValueAsync(string context, string key, string oldAugumentedKey, string value)
        {
            throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "This secure store is read-only");
        }

        public async Task RemoveValueAsync(string context, string key)
        {
            throw new SecureStoreException(SecureStoreException.Type.UnsupportedOperation, "This secure store is read-only");
        }
    }
}
