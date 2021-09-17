using dotenv.net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UiPath.Orchestrator.BeyondTrustDynamicSystemReadOnly;
using UiPath.Orchestrator.BeyondTrustSingleSystemReadOnly;
using UiPath.Orchestrator.BeyondTrustTeamPasswordsReadOnly;

namespace BeyondTrust_UnitTest.ReadOnly
{
    [TestClass]
    public class CredentialTest
    {
        private readonly BeyondTrustSingleSystemSecureStore singleSystemSecureStore = new();
        private readonly BeyondTrustDynamicSystemSecureStore dynamicSystemSecureStore = new();
        private readonly BeyondTrustTeamPasswordsSecureStore teamPasswordSecureStore = new();
        private static Dictionary<string, object> config;
        private static IDictionary<string, string> envVars;

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            DotEnv.Load();
            envVars = DotEnv.Read();
            config = new()
            {
                { "Hostname", envVars["HOSTNAME"] },
                { "AuthKey", envVars["AUTH_KEY"] },
                { "RunAs", envVars["RUN_AS"] },
                { "SSLEnabled", false },
            };
        }

        // ------------- SINGLE SYSTEM -------------
        [TestMethod]
        public async Task GetSingleSystemCredential()
        {
            try
            {
                config.Add("ManagedSystemName", envVars["MANAGED_SYSTEM"]);
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await singleSystemSecureStore.GetCredentialsAsync(context, envVars["SYSTEM_MANAGED_ACCOUNT"]);

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.AreEqual(credential.Username, "");
            Assert.AreEqual(credential.Password, "");
        }

        [TestMethod]
        public async Task GetSingleSystemCredentialTwiceFast()
        {
            Console.WriteLine("RUN 1");
            await GetSingleSystemCredential();
            Console.WriteLine("RUN 2");
            await GetSingleSystemCredential();
        }

        [TestMethod]
        public async Task GetSingleSystemValue()
        {
            try
            {
                config.Add("ManagedSystemName", envVars["MANAGED_SYSTEM"]);
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var value = await singleSystemSecureStore.GetValueAsync(context, envVars["SYSTEM_MANAGED_ACCOUNT"]);
            Assert.AreEqual(value, "");
        }

        [TestMethod]
        public async Task GetSingleSystemADCredential()
        {
            try
            {
                config.Add("ManagedSystemName", envVars["MANAGED_SYSTEM"]);
                config.Add("ManagedAccountType", "domainlinked");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await singleSystemSecureStore.GetCredentialsAsync(context, envVars["AD_MANAGED_ACCOUNT"]);

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.IsNotNull(credential.Username);
            Assert.IsNotNull(credential.Password);
        }

        // ------------- DYNAMIC SYSTEM -------------
        [TestMethod]
        public async Task GetDynamicSystemCredential()
        {
            try
            {
                config.Add("SystemAccountDelimiter", "/");
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await dynamicSystemSecureStore.GetCredentialsAsync(context, envVars["MANAGED_SYSTEM"] + "/" + envVars["SYSTEM_MANAGED_ACCOUNT"]);

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.AreEqual(credential.Username, "");
            Assert.AreEqual(credential.Password, "");
        }

        [TestMethod]
        public async Task GetDynamicSystemCredentialTwiceFast()
        {
            Console.WriteLine("RUN 1");
            await GetDynamicSystemCredential();
            Console.WriteLine("RUN 2");
            await GetDynamicSystemCredential();
        }

        [TestMethod]
        public async Task GetDynamicSystemValue()
        {
            try
            {
                config.Add("SystemAccountDelimiter", "/");
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var value = await dynamicSystemSecureStore.GetValueAsync(context, envVars["MANAGED_SYSTEM"] + "/" + envVars["SYSTEM_MANAGED_ACCOUNT"]);
            Assert.AreEqual(value, "");
        }

        [TestMethod]
        public async Task GetDynamicSystemADCredential()
        {
            try
            {
                config.Add("SystemAccountDelimiter", "/");
                config.Add("ManagedAccountType", "domainlinked");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await dynamicSystemSecureStore.GetCredentialsAsync(context, envVars["MANAGED_SYSTEM"] + "/" + envVars["AD_MANAGED_ACCOUNT"]);

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.IsNotNull(credential.Username);
            Assert.IsNotNull(credential.Password);
        }

        // ------------- TEAM PASSWORDS -------------
        [TestMethod]
        public async Task GetTeamPassword()
        {
            try
            {
                config.Add("FolderPasswordDelimiter", "/");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await teamPasswordSecureStore.GetCredentialsAsync(context, envVars["TEAM_PASSWORDS_FOLDER"] + "/" + envVars["TEAM_PASSWORD_TITLE"]);

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.AreEqual("testuser", credential.Username);
            Assert.AreEqual("testpassword", credential.Password);
        }
    }
}
