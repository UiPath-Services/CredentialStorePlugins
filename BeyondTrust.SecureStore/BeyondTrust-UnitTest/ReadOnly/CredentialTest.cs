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
        private readonly Dictionary<string, object> config = new()
        {
            { "Hostname", "https://ritjnmzm.ps.beyondtrustcloud.com" },
            { "AuthKey", "b6cd8ea8afe98e6fde1200780dc465a325ef015751414a37d71b27123e4bbd9c7c43cde389be1c3d82c8b8ec24753099b81623e2c121a8059edc01028591006e" },
            { "RunAs", "uipath" },
            { "SSLEnabled", false },
        };

        // ------------- SINGLE SYSTEM -------------
        [TestMethod]
        public async Task GetSingleSystemCredential()
        {
            try
            {
                config.Add("ManagedSystemName", "btlab.btu.cloud");
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await singleSystemSecureStore.GetCredentialsAsync(context, "botuser01");

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
                config.Add("ManagedSystemName", "btlab.btu.cloud");
                config.Add("ManagedAccountType", "system");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var value = await singleSystemSecureStore.GetValueAsync(context, "botuser01");
            Assert.AreEqual(value, "");
        }

        [TestMethod]
        public async Task GetSingleSystemADCredential()
        {
            try
            {
                config.Add("ManagedSystemName", "btlab.btu.cloud");
                config.Add("ManagedAccountType", "domainlinked");
            }
            catch (Exception)
            {
                // Continue if keys already added
            }
            var context = JsonConvert.SerializeObject(config);
            var credential = await singleSystemSecureStore.GetCredentialsAsync(context, "botuser01");

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
            var credential = await dynamicSystemSecureStore.GetCredentialsAsync(context, "btlab.btu.cloud/botuser01");

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
            var value = await dynamicSystemSecureStore.GetValueAsync(context, "btlab.btu.cloud/botuser01");
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
            var credential = await dynamicSystemSecureStore.GetCredentialsAsync(context, "btlab.btu.cloud/botuser01");

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
            var credential = await teamPasswordSecureStore.GetCredentialsAsync(context, "UiPath/TestCredential");

            Console.WriteLine(credential.Username + "; " + credential.Password);
            Assert.AreEqual("testuser", credential.Username);
            Assert.AreEqual("testpassword", credential.Password);
        }
    }
}
