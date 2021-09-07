using dotenv.net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard;

namespace UiPath.Samples.SecureStores.OneIdentitySafeguard
{
    [TestClass]
    public class OneIdentitySafeguardSecureStoreTests
    {
        private IDictionary<string, string> _envVars;
        private OneIdentitySafeguardSecureStore _store;
        private OneIdentitySafeguardContext _context;

        [TestInitialize]
        public void Init()
        {
            DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { Environment.CurrentDirectory }));
            _envVars = DotEnv.Read();
            _store = new OneIdentitySafeguardSecureStore();
            _context = new OneIdentitySafeguardContext
            {
                Hostname = _envVars["HOSTNAME"],
                Thumbprint = _envVars["THUMBPRINT"],
                IgnoreSSL = true,
            };
        }

        [TestMethod]
        public async Task GetCredential()
        {
            var credential = await _store.GetCredentialsAsync(JsonConvert.SerializeObject(_context), _envVars["API_KEY"]);
            Console.WriteLine("Retrieved password: " + credential.Password);
            Assert.AreEqual(credential.Password, _envVars["EXPECTED_PASSWORD"]);
        }

        [TestMethod]
        public async Task GetValue()
        {
            var password = await _store.GetValueAsync(JsonConvert.SerializeObject(_context), _envVars["API_KEY"]);
            Console.WriteLine("Retrieved password: " + password);
            Assert.AreEqual(password, _envVars["EXPECTED_PASSWORD"]);
        }

        [TestMethod]
        public async Task GetValueTwice()
        {
            var password = await _store.GetValueAsync(JsonConvert.SerializeObject(_context), _envVars["API_KEY"]);
            Console.WriteLine("Retrieved password: " + password);
            Assert.AreEqual(password, _envVars["EXPECTED_PASSWORD"]);
            var password2 = await _store.GetValueAsync(JsonConvert.SerializeObject(_context), _envVars["API_KEY"]);
            Console.WriteLine("Second retrieved password: " + password2);
            Assert.AreEqual(password2, _envVars["EXPECTED_PASSWORD"]);
        }
    }
}