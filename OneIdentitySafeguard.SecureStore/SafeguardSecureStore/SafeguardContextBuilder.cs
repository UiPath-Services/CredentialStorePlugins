using Newtonsoft.Json;
using System;
using UiPath.Orchestrator.Extensibility.SecureStores;
using UiPath.Orchestrator.SafeguardSecureStore;

namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public class SafeguardContextBuilder
    {
        private SafeguardContext _context;

        public SafeguardContextBuilder FromJson(string json)
        {
            json = json ?? throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardJsonInvalidOrMissing), json));

            try
            {
                _context = JsonConvert.DeserializeObject<SafeguardContext>(json);
            }
            catch (Exception)
            {
                throw new Exception(
                        SafeguardUtils.GetLocalizedResource(nameof(Resource.JsonDeserializationIssue), json, _context.SafeguardAppliance, _context.SafeguardCertThumbprint, _context.IgnoreSSL));

            }

            return this;
        }



        public SafeguardContext Build()
        {
            if (_context == null)
            {
                throw new Exception("Invalid usage");
            }

            if (string.IsNullOrEmpty(_context.SafeguardAppliance))
            {
                throw new SecureStoreException(
                    SecureStoreException.Type.InvalidConfiguration,
                    SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardSettingInvalidOrMissing), _context.SafeguardAppliance, "..." + _context.SafeguardCertThumbprint.Substring(36), _context.IgnoreSSL));

            }

            if (string.IsNullOrEmpty(_context.SafeguardCertThumbprint))
            {

                throw new SecureStoreException(
                    SecureStoreException.Type.InvalidConfiguration,
                    SafeguardUtils.GetLocalizedResource(nameof(Resource.SafeguardSettingInvalidOrMissing), _context.SafeguardAppliance, "..." + _context.SafeguardCertThumbprint.Substring(36), _context.IgnoreSSL));

            }

            return _context;
        }
    }
}