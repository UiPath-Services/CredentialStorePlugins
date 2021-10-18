using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BeyondTrust.BeyondInsight.PasswordSafe.API.Client.V3
{
    public sealed class TeamPasswordsCredentialsEndpoint : BaseEndpoint
    {
        internal TeamPasswordsCredentialsEndpoint(PasswordSafeAPIConnector client)
            : base(client)
        {
        }

        /// <summary>
        /// List of Team Passwords Credential model versions.
        /// </summary>
        public static List<string> Versions = new List<string>() { v30, v31 };
        public const string v30 = "3.0";
        public const string v31 = "3.1";

        /// <summary>
        /// Returns a list of Team Passwords Credentials by Team Passwords Folder ID.
        /// <para>API: GET TeamPasswords/Folders/{id}/Credentials</para>
        /// </summary>
        /// <param name="folderID">ID of the Team Passwords Folder</param>
        public TeamPasswordsCredentialsResult GetAll(Guid folderID)
        {
            HttpResponseMessage response = _conn.Get($"TeamPasswords/Folders/{folderID}/Credentials");
            TeamPasswordsCredentialsResult result = new TeamPasswordsCredentialsResult(response);
            return result;
        }

        /// <summary>
        /// Returns a Team Passwords Credential by ID.
        /// <para>API: GET TeamPasswords/Credentials/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Team Passwords Credential</param>
        public TeamPasswordsCredentialWithPasswordResult Get(Guid id)
        {
            HttpResponseMessage response = _conn.Get($"TeamPasswords/Credentials/{id}");
            TeamPasswordsCredentialWithPasswordResult result = new TeamPasswordsCredentialWithPasswordResult(response);
            return result;
        }

        /// <summary>
        /// Creates a new Team Passwords Credential by Team Passwords Folder ID.
        /// <para>API: POST TeamPasswords/Folders/{id}/Credentials</para>
        /// </summary>
        /// <param name="folderID">ID of the Team Passwords Folder</param>
        /// <param name="version">The model version</param>
        public TeamPasswordsCredentialResult Post(Guid folderID, TeamPasswordsCredentialPostModel model, string version = v30)
        {
            HttpResponseMessage response = _conn.Post($"TeamPasswords/Folders/{folderID}/Credentials/?version={version}", model);
            TeamPasswordsCredentialResult result = new TeamPasswordsCredentialResult(response);
            return result;
        }

        /// <summary>
        /// Updates an existing Team Passwords Credential by ID.
        /// <para>API: PUT TeamPasswords/Credentials/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Team Passwords Credential</param>
        /// <param name="version">The model version</param>
        public TeamPasswordsCredentialResult Put(Guid id, TeamPasswordsCredentialWithPasswordModel model, string version = v30)
        {
            HttpResponseMessage response = _conn.Put($"TeamPasswords/Credentials/{id}/?version={version}", model);
            TeamPasswordsCredentialResult result = new TeamPasswordsCredentialResult(response);
            return result;
        }

        /// <summary>
        /// Deletes a Team Passwords Credential by ID.
        /// <para>API: DELETE TeamPasswords/Credentials/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Team Passwords Credential</param>
        public DeleteResult Delete(Guid id)
        {
            HttpResponseMessage response = _conn.Delete($"TeamPasswords/Credentials/{id}");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

    }
}
