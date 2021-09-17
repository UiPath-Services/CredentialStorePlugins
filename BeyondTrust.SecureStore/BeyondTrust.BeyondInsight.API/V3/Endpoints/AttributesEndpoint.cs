using System.Net.Http;

namespace BeyondTrust.BeyondInsight.PasswordSafe.API.Client.V3
{
    public sealed class AttributesEndpoint : BaseEndpoint
    {
        internal AttributesEndpoint(PasswordSafeAPIConnector client)
            : base(client)
        {
        }

        #region Attribute Definitions

        /// <summary>
        /// Returns a list of Attribute definitions by Attribute Type.
        /// <para>API: GET AttributeTypes/{attributeTypeID}/Attributes</para>
        /// </summary>
        /// <param name="attributeTypeID">ID of the Attribute Type</param>
        /// <returns></returns>
        public AttributesResult GetAll(int attributeTypeID)
        {
            HttpResponseMessage response = _conn.Get(string.Format("AttributeTypes/{0}/Attributes", attributeTypeID));
            AttributesResult result = new AttributesResult(response);
            return result;
        }

        /// <summary>
        /// Returns an Attribute definition by ID.
        /// <para>API: GET Attributes/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Attribute</param>
        /// <returns></returns>
        public AttributeResult Get(int id)
        {
            HttpResponseMessage response = _conn.Get(string.Format("Attributes/{0}", id));
            AttributeResult result = new AttributeResult(response);
            return result;
        }

        /// <summary>
        /// Creates a new Attribute definition by Attribute Type ID.
        /// <para>API: POST AttributeTypes/{attributeTypeID}/Attributes</para>
        /// </summary>
        /// <param name="attributeTypeID">ID of the Attribute Type</param>
        /// <returns></returns>
        public AttributeResult Post(int attributeTypeID, AttributePostModel model)
        {
            HttpResponseMessage response = _conn.Post(string.Format("AttributeTypes/{0}/Attributes", attributeTypeID), model);
            AttributeResult result = new AttributeResult(response);
            return result;
        }

        /// <summary>
        /// Deletes an Attribute definition by ID.
        /// <para>API: DELETE Attributes/{id}</para>
        /// </summary>
        /// <param name="id">ID of the Attribute</param>
        /// <returns></returns>
        public DeleteResult Delete(int id)
        {
            HttpResponseMessage response = _conn.Delete(string.Format("Attributes/{0}", id));
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        #endregion

        #region Asset Attributes

        /// <summary>
        /// Returns a list of Attributes by Asset ID.
        /// <para>API: GET Assets/{assetID}/Attributes</para>
        /// </summary>
        /// <param name="assetID">ID of the Asset</param>
        public AttributesResult GetAllByAsset(int assetID)
        {
            HttpResponseMessage response = _conn.Get($"Assets/{assetID}/Attributes");
            AttributesResult result = new AttributesResult(response);
            return result;
        }

        /// <summary>
        /// Assigns an Attribute to an Asset.
        /// <para>API: POST Assets/{assetID}/Attributes/{attributeID}</para>
        /// </summary>
        /// <param name="assetID">ID of the Asset</param>
        /// <param name="attributeID">ID of the Attribute</param>
        public AttributeResult PostByAsset(int assetID, int attributeID)
        {
            HttpResponseMessage response = _conn.Post($"Assets/{assetID}/Attributes/{attributeID}");
            AttributeResult result = new AttributeResult(response);
            return result;
        }

        /// <summary>
        /// Deletes all Asset Attributes by Asset ID.
        /// <para>API: DELETE Assets/{id}/Attributes</para>
        /// </summary>
        /// <param name="assetID">ID of the Asset</param>
        public DeleteResult DeleteAllByAsset(int assetID)
        {
            HttpResponseMessage response = _conn.Delete($"Assets/{assetID}/Attributes");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        /// <summary>
        /// Deletes an Asset Attribute by Asset ID and Attribute ID.
        /// <para>API: DELETE Attributes/{id}</para>
        /// </summary>
        /// <param name="assetID">ID of the Asset</param>
        /// <param name="attributeID">ID of the Attribute</param>
        public DeleteResult DeleteByAsset(int assetID, int attributeID)
        {
            HttpResponseMessage response = _conn.Delete($"Assets/{assetID}/Attributes/{attributeID}");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        #endregion


        #region Managed System Attributes

        /// <summary>
        /// Returns a list of Attributes by Managed System ID.
        /// <para>API: GET ManagedSystems/{managedSystemID}/Attributes</para>
        /// </summary>
        /// <param name="managedSystemID">ID of the Managed System</param>
        public AttributesResult GetAllByManagedSystem(int managedSystemID)
        {
            HttpResponseMessage response = _conn.Get($"ManagedSystems/{managedSystemID}/Attributes");
            AttributesResult result = new AttributesResult(response);
            return result;
        }

        /// <summary>
        /// Assigns an Attribute to a Managed System.
        /// <para>API: POST ManagedSystems/{managedSystemID}/Attributes/{attributeID}</para>
        /// </summary>
        /// <param name="managedSystemID">ID of the Managed System</param>
        /// <param name="attributeID">ID of the Attribute</param>
        public AttributeResult PostByManagedSystem(int managedSystemID, int attributeID)
        {
            HttpResponseMessage response = _conn.Post($"ManagedSystems/{managedSystemID}/Attributes/{attributeID}");
            AttributeResult result = new AttributeResult(response);
            return result;
        }

        /// <summary>
        /// Deletes all Managed System Attributes by Managed System ID.
        /// <para>API: DELETE ManagedSystems/{id}/Attributes</para>
        /// </summary>
        /// <param name="managedSystemID">ID of the Managed System</param>
        public DeleteResult DeleteAllByManagedSystem(int managedSystemID)
        {
            HttpResponseMessage response = _conn.Delete($"ManagedSystems/{managedSystemID}/Attributes");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        /// <summary>
        /// Deletes a Managed System Attribute by Managed System ID and Attribute ID.
        /// <para>API: DELETE ManagedSystems/{managedSystemID}/Attributes/{attributeID}</para>
        /// </summary>
        /// <param name="managedSystemID">ID of the Managed System</param>
        /// <param name="attributeID">ID of the Attribute</param>
        public DeleteResult DeleteByManagedSystem(int managedSystemID, int attributeID)
        {
            HttpResponseMessage response = _conn.Delete($"ManagedSystems/{managedSystemID}/Attributes/{attributeID}");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        #endregion

        #region Managed Account Attributes

        /// <summary>
        /// Returns a list of Attributes by Managed Account ID.
        /// <para>API: GET ManagedAccounts/{managedAccountID}/Attributes</para>
        /// </summary>
        /// <param name="managedAccountID">ID of the Managed Account</param>
        public AttributesResult GetAllByManagedAccount(int managedAccountID)
        {
            HttpResponseMessage response = _conn.Get($"ManagedAccounts/{managedAccountID}/Attributes");
            AttributesResult result = new AttributesResult(response);
            return result;
        }

        /// <summary>
        /// Assigns an Attribute to a Managed Account.
        /// <para>API: POST ManagedAccounts/{managedAccountID}/Attributes/{attributeID}</para>
        /// </summary>
        /// <param name="managedAccountID">ID of the Managed Account</param>
        /// <param name="attributeID">ID of the Attribute</param>
        public AttributeResult PostByManagedAccount(int managedAccountID, int attributeID)
        {
            HttpResponseMessage response = _conn.Post($"ManagedAccounts/{managedAccountID}/Attributes/{attributeID}");
            AttributeResult result = new AttributeResult(response);
            return result;
        }

        /// <summary>
        /// Deletes all Managed Account Attributes by Managed Account ID.
        /// <para>API: DELETE ManagedAccounts/{id}/Attributes</para>
        /// </summary>
        /// <param name="managedAccountID">ID of the Managed Account</param>
        public DeleteResult DeleteAllByManagedAccount(int managedAccountID)
        {
            HttpResponseMessage response = _conn.Delete($"ManagedAccounts/{managedAccountID}/Attributes");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        /// <summary>
        /// Deletes a Managed Account Attribute by Managed Account ID and Attribute ID.
        /// <para>API: DELETE ManagedAccounts/{managedAccountID}/Attributes/{attributeID}</para>
        /// </summary>
        /// <param name="managedAccountID">ID of the Managed Account</param>
        /// <param name="attributeID">ID of the Attribute</param>
        public DeleteResult DeleteByManagedAccount(int managedAccountID, int attributeID)
        {
            HttpResponseMessage response = _conn.Delete($"ManagedAccounts/{managedAccountID}/Attributes/{attributeID}");
            DeleteResult result = new DeleteResult(response);
            return result;
        }

        #endregion

    }
}
