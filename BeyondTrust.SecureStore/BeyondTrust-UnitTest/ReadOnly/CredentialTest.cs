using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UiPath.Orchestrator.BeyondTrustDynamicSystemReadOnly;
using UiPath.Orchestrator.BeyondTrustSingleSystemReadOnly;

namespace BeyondTrust_UnitTest.ReadOnly
{
    [TestClass]
    public class CredentialTest
    {
        private readonly BeyondTrustSingleSystemSecureStore singleSystemSecureStore = new BeyondTrustSingleSystemSecureStore();
        private readonly BeyondTrustDynamicSystemSecureStore dynamicSystemSecureStore = new BeyondTrustDynamicSystemSecureStore();
        private readonly Dictionary<string, object> config = new Dictionary<string, object>
            {
                { "Hostname", "" },
                {"AuthKey", "" },
                {"RunAs", "" },
                {"SSLEnabled", false },
            };

        // ------------- SINGLE SYSTEM -------------
        [TestMethod]
        public async Task GetSingleSystemCredential()
        {
            try
            {
                config.Add("ManagedSystemName", "");
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await singleSystemSecureStore.GetCredentialsAsync(context, "");

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
                config.Add("ManagedSystemName", "");
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var value = await singleSystemSecureStore.GetValueAsync(context, "");
            Assert.AreEqual(value, "");
        }

        [TestMethod]
        public async Task GetSingleSystemADCredential()
        {
            try
            {
                config.Add("ManagedSystemName", "");
                config.Add("ManagedAccountType", "domainlinked");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await singleSystemSecureStore.GetCredentialsAsync(context, "");

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
            var credential = await dynamicSystemSecureStore.GetCredentialsAsync(context, "");

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
            var value = await dynamicSystemSecureStore.GetValueAsync(context, "");
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
            var credential = await dynamicSystemSecureStore.GetCredentialsAsync(context, "");

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.IsNotNull(credential.Username);
            Assert.IsNotNull(credential.Password);
        }
    }
}
