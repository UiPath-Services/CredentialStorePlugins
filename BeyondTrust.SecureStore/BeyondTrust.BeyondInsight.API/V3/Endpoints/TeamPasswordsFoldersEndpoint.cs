using System;
using System.Net.Http;

namespace BeyondTrust.BeyondInsight.PasswordSafe.API.Client.V3
{
    public sealed class TeamPasswordsFoldersEndpoint : BaseEndpoint
    {
        internal TeamPasswordsFoldersEndpoint(PasswordSafeAPIConnector client)
            : base(client)
        {
        }

        /// <summary>
        /// Returns a list of Team Passwords Folders.
        /// <para>API: GET TeamPasswords/Folders</para>
        /// </summary>
        public TeamPasswordsFoldersResult GetAll()
        {
            HttpResponseMessage response = _conn.Get("TeamPasswords/Folders");
            TeamPasswordsFoldersResult result = new TeamPasswordsFoldersResult(response);
            return result;
        }

        /// <summary>
        /// Returns a Team Passwords Folder by ID.
        /// <para>API: GET TeamPasswords/Folders/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Team Passwords Folder</param>
        public TeamPasswordsFolderResult Get(Guid id)
        {
            HttpResponseMessage response = _conn.Get($"TeamPasswords/Folders/{id}");
            TeamPasswordsFolderResult result = new TeamPasswordsFolderResult(response);
            return result;
        }

        /// <summary>
        /// Creates a new Team Passwords Folder.
        /// <para>API: POST TeamPasswords/Folders</para>
        /// </summary>
        public TeamPasswordsFolderResult Post(TeamPasswordsFolderPostModel model)
        {
            HttpResponseMessage response = _conn.Post("TeamPasswords/Folders", model);
            TeamPasswordsFolderResult result = new TeamPasswordsFolderResult(response);
            return result;
        }

        /// <summary>
        /// Updates an existing Team Passwords Folder by ID.
        /// <para>API: PUT TeamPasswords/Folders/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Team Passwords Folder</param>
        public TeamPasswordsFolderResult Put(Guid id, TeamPasswordsFolderModel model)
        {
            HttpResponseMessage response = _conn.Put($"TeamPasswords/Folders/{id}", model);
            TeamPasswordsFolderResult result = new TeamPasswordsFolderResult(response);
            return result;
        }

        /// <summary>
        /// Deletes a Team Passwords Folder and all related Credentials by ID.
        /// <para>API: DELETE TeamPasswords/Folders/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Team Passwords Folder</param>
        public DeleteResult Delete(Guid id)
        {
            HttpResponseMessage response = _conn.Delete($"TeamPasswords/Folders/{id}");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

    }
}
