using System.Net.Http;

namespace BeyondTrust.BeyondInsight.PasswordSafe.API.Client.V3
{
    public sealed class AuthEndpoint : BaseEndpoint
    {
        internal AuthEndpoint(PasswordSafeAPIConnector client)
            : base(client)
        {
        }

        /// <summary>
        /// The model of the authenticated user.  If IsAuthenticated is false, returns null.
        /// </summary>
        public SignAppInUserModel User
        {
            get { return _conn.User; }
            internal set { _conn.User = value; }
        }

        /// <summary>
        /// Returns whether the client is authenticated with the server.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return User != null; }
        }

        /// <summary>
        /// Authenticates with the server using the given Application API Key and username.
        /// <para>API: POST Auth/SignAppin</para>
        /// </summary>
        /// <param name="apiKey">The Application API Key.</param>
        /// <param name="username">The username of the user.</param>
        /// <returns></returns>
        public AuthenticationResult SignAppIn(string apiKey, string username)
        {
            _conn.Reset();

            // Use App API Key to Authenticate User
            HttpResponseMessage response = _conn.Auth("Auth/SignAppin", "Authorization", $"PS-Auth key={apiKey}; runas={username};");
            return ProcessAuthenticationResult(response);
        }
        
        /// <summary>
        /// Transforms the HttpResponseMessage into an AuthenticationResult and sets local properties on success.
        /// </summary>
        /// <param name="response">The response of the request.</param>
        private AuthenticationResult ProcessAuthenticationResult(HttpResponseMessage response)
        {
            AuthenticationResult result = new AuthenticationResult(response);
            if (result.IsSuccess)
                User = result.Value;

            return result;
        }

        /// <summary>
        /// Signs out of an authenticated connection.
        /// <para>API: Auth/Signout</para>
        /// </summary>
        /// <returns></returns>
        public SignOutResult SignOut()
        {
            HttpResponseMessage response = _conn.Post("Auth/Signout");
            SignOutResult result = new SignOutResult(response);

            // if successful, user is no longer authenticated
            if (result.IsSuccess)
                User = null;

            return result;
        }

    }
}
