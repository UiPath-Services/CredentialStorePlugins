using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace BeyondTrust.BeyondInsight.PasswordSafe.API.Client.V3
{
    /// <summary>
    /// Password Safe API v3 Connector.
    /// </summary>
    internal class PasswordSafeAPIConnector
    {
        /// <summary>
        /// The base URL of the API endpoints.
        /// </summary>
        private readonly string _Url = string.Empty;

        /// <summary>
        /// The HttpClient used to communicate with the server.
        /// </summary>
        private readonly HttpClient _HttpClient;

        /// <summary>
        /// Constructor for <seealso cref="PasswordSafeAPIConnector"/>.
        /// </summary>
        /// <param name="baseUrl">
        /// The base URL of the Password Safe API.
        /// <para>i.e. <example>https://the-url/BeyondTrust/api/public/v3</example></para>
        /// </param>
        internal PasswordSafeAPIConnector(string baseUrl, bool ignoreSSLWarning, X509Certificate2 clientCert)
        {
            _Url = baseUrl;
            _HttpClient = InitClient();

            HttpClient InitClient()
            {
                HttpClientHandler handler = new HttpClientHandler();

                if (ignoreSSLWarning)
                    handler.ServerCertificateCustomValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                if (clientCert != null)
                    handler.ClientCertificates.Add(clientCert);

                return new HttpClient(handler);
            }
        }

        #region Internal Properties

        /// <summary>
        /// The model of the authenticated user.  Returns null if not authenticated.
        /// </summary>
        internal SignAppInUserModel User { get; set; } = null;

        #endregion

        #region Init

        /// <summary>
        /// Resets the client.
        /// </summary>
        internal void Reset()
        {
            User = null;
        }

        #endregion

        #region Http Utilities

        /// <summary>
        /// Builds and returns an absolute Uri for an API.
        /// </summary>
        /// <param name="api">The name of the API for the Uri.</param>
        private Uri BuildUri(string api)
        {
            string suri = $"{_Url}/{api}";
            Uri uri = new Uri(suri);
            return uri;
        }

        /// <summary>
        /// Send a POST request to the given API, with the specified <paramref name="headerName"/> and <paramref name="headerValue"/>.  Typically used for Authentication.
        /// </summary>
        /// <param name="api">The name of the POST API.</param>
        /// <param name="headerName">The header name to add to the POST request.</param>
        /// <param name="headerValue">The header value to add to the POST request.</param>
        internal HttpResponseMessage Auth(string api, string headerName, string headerValue)
        {
            if (string.IsNullOrEmpty(headerName)) throw new ArgumentNullException(nameof(headerName));

            Uri uri = BuildUri(api);
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = Utilities.SerializeContent(null)
            };

            msg.Headers.Add(headerName, headerValue);

            HttpResponseMessage response = _HttpClient.SendAsync(msg).Result;

            // if unauthorized, reset
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                Reset();

            return response;
        }

        /// <summary>
        /// Send a GET request to the given API.
        /// </summary>
        /// <param name="api">The name of the GET API.</param>
        internal HttpResponseMessage Get(string api)
        {
            Uri uri = BuildUri(api);
            HttpResponseMessage response = _HttpClient.GetAsync(uri).Result;

            // if unauthorized, reset
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                Reset();

            return response;
        }

        /// <summary>
        /// Send a POST request to the given API with no post content.
        /// </summary>
        /// <param name="api">The name of the POST API.</param>
        internal HttpResponseMessage Post(string api)
        {
            return Post(api, null);
        }

        /// <summary>
        /// Send a POST request to the given API, serializing the postContent.
        /// </summary>
        /// <param name="api">The name of the POST API.</param>
        /// <param name="postContent">The content to serialize for the POST request.</param>
        internal HttpResponseMessage Post(string api, object postContent, string headerName = null, string headerValue = null)
        {
            Uri uri = BuildUri(api);
            StringContent content = Utilities.SerializeContent(postContent);

            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, uri)
            { 
                Content = content 
            };
            if (!string.IsNullOrEmpty(headerName))
                msg.Headers.Add(headerName, headerValue);

            HttpResponseMessage response = _HttpClient.SendAsync(msg).Result;

            // if unauthorized, reset
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                Reset();

            return response;
        }

        /// <summary>
        /// Send a PUT request to the given API, serializing the putContent.
        /// </summary>
        /// <param name="api">The name of the PUT API.</param>
        /// <param name="postContent">The content to serialize for the PUT request.</param>
        internal HttpResponseMessage Put(string api, object putContent)
        {
            Uri uri = BuildUri(api);
            StringContent content = Utilities.SerializeContent(putContent);
            HttpResponseMessage response = _HttpClient.PutAsync(uri, content).Result;

            // if unauthorized, reset
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                Reset();

            return response;
        }

        /// <summary>
        /// Send a DELETE request to the given API.
        /// </summary>
        /// <param name="api">The name of the DELETE API.</param>
        internal HttpResponseMessage Delete(string api)
        {
            Uri uri = BuildUri(api);
            HttpResponseMessage response = _HttpClient.DeleteAsync(uri).Result;

            // if unauthorized, reset
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                Reset();

            return response;
        }

        #endregion

    }

}
