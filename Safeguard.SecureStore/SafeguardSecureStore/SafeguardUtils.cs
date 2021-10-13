
using System.Globalization;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using UiPath.Orchestrator.Extensibility.SecureStores;
using UiPath.Orchestrator.SafeguardSecureStore;

namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public static class SafeguardUtils
    {
        
       public static Dictionary<string,string> ExtractKey (string key)
        {
            var SafeguardA2ATarget = new Dictionary<string, string>()
            {
                { "SafeguardAsset", string.Empty},
                { "SafeguardAccount", string.Empty},
                { "SafeguardAPIKey", string.Empty},
                { "SafeguardA2AMethod", string.Empty}
            };
            if (key.StartsWith("a2akey:"))
            {
                SafeguardA2ATarget["SafeguardAPIKey"] = key.Substring(7);
                SafeguardA2ATarget["SafeguardA2AMethod"] = "a2akey";
                SafeguardA2ATarget["SafeguardAccount"] = "n/a";
            }
            else if (key.Contains("@"))
            {
                SafeguardA2ATarget["SafeguardAsset"] = key.Split('@')[1];
                SafeguardA2ATarget["SafeguardAccount"] = key.Split('@')[0];
                SafeguardA2ATarget["SafeguardA2AMethod"] = "account_lookup";
            }
            else if (key.Contains("\\"))
            {
                SafeguardA2ATarget["SafeguardAsset"] = key.Split('\\')[0];
                SafeguardA2ATarget["SafeguardAccount"] = key.Split('\\')[1];
                SafeguardA2ATarget["SafeguardA2AMethod"] = "account_lookup";
            }
            else throw new SecureStoreException(
                SecureStoreException.Type.InvalidConfiguration,
                "Target or API key can't be extracted");
            return SafeguardA2ATarget;
        }
        public static string GetLocalizedResource(string resourceName, params object[] parameters)
        {
            var resource = GetLocalizedResource(Thread.CurrentThread.CurrentUICulture, resourceName, parameters);
            if (!string.IsNullOrEmpty(resource))
            {
                return resource;
            }

            return GetLocalizedResource(CultureInfo.InvariantCulture, resourceName, parameters);
        }

        private static string GetLocalizedResource(CultureInfo cultureInfo, string resourceName, params object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                var message = Resource.ResourceManager.GetString(resourceName, cultureInfo);

                return string.IsNullOrEmpty(message) ? null : string.Format(message, parameters);
            }

            return Resource.ResourceManager.GetString(resourceName, cultureInfo);
        }
    }
}