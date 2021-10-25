using Xunit;
using UiPath.Orchestrator.Extensions.SecureStores.Safeguard;
using System.Collections.Generic;
using UiPath.Orchestrator.Extensibility.SecureStores;
using FluentAssertions;
using UiPath.Orchestrator.Extensibility.Configuration;
using UiPath.Orchestrator.SafeguardSecureStoreTests;
using System.Diagnostics;

namespace UiPath.Samples.SecureStores.SafeguardStore
{


    public class SafeguardSecureStoreTests
    {

        private const string SafeguardAppliance = "";
        private const string SafeguardCertThumbprint = "";
        private const string IgnoreSSL = "true";
        private const string ContextSafeguard = "{\"SafeguardAppliance\": \"" + SafeguardAppliance + "\", \"SafeguardCertThumbprint\": \"" + SafeguardCertThumbprint + "\", \"IgnoreSSL\": " + IgnoreSSL + "}";
        private const string KeyAsset = "";
        private const string KeyDomain = "";
        private const string KeyApiKey = "";
        private const string PasswordAssetUser = "";
        private const string PasswordDomainUser = "";


        private readonly SafeguardSecureStore _sgStore = new SafeguardSecureStore();

        [Fact]
        public void ValidateContextAsync()
        {
            _sgStore.ValidateContextAsync(ContextSafeguard);
        }


        [Fact]
        public void Initialize()
        {

            if (!EventLog.SourceExists("Orchestrator") && DebugLogging.Equals("true"))
            {
                EventLog.CreateEventSource("Orchestrator", "Application");
            }
            _sgStore.Initialize(new Dictionary<string, string>());
        }

        [Fact]
        public void GetStoreInfoReturnsCorrectObject()
        {
            var expected = new SecureStoreInfo
            {
                Identifier = "One Identity Safeguard",
                IsReadOnly = true,
            };

            var actual = _sgStore.GetStoreInfo();
            actual.Should().BeEquivalentTo(expected);
        }


        /*[Fact]
        public void ValidateContextAsyncSucceedsGivenValidJsonString()
        {
            _sgStore.ValidateContextAsync(ContextCanFailTrue);
        }

        [Fact]
        public async void ValidateContextAsyncFailsGivenInvalidJsonString()
        {
            var ex = await Assert.ThrowsAsync<JsonReaderException>(
                () => _sgStore.ValidateContextAsync(NotJsonString));
        }
        */

        [Fact]
        public void GetConfigurationReturnsCorrectObject()
        {
            var expected = new List<ConfigurationEntry>
            {
                new ConfigurationValue(ConfigurationValueType.String)
                {
                    Key = "SafeguardAppliance",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingSafeguardAppliance)),
                    IsMandatory = true
                },
                new ConfigurationValue(ConfigurationValueType.Secret)
                {
                    Key = "SafeguardCertThumbprint",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingSafeguardCertificateThumprint)),
                    IsMandatory = true
                },
                new ConfigurationValue(ConfigurationValueType.Boolean)
                {
                    Key = "IgnoreSSL",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingIgnoreSSL)),
                    IsMandatory = true
                },
                new ConfigurationValue(ConfigurationValueType.Boolean)
                {
                    Key = "DebugLogging",
                    DisplayName = SafeguardUtils.GetLocalizedResource(nameof(Resource.SettingDebugLogging)),
                    IsMandatory = true
                },
            };

            var actual = _sgStore.GetConfiguration();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void GetValueAsyncAsset()
        {
            string expectedPassword = PasswordAssetUser;
            string actualPassword = await _sgStore.GetValueAsync(ContextSafeguard, KeyAsset);
            actualPassword.Should().BeEquivalentTo(expectedPassword);
        }

        [Fact]
        public async void GetValueAsyncDomain()
        {
            string expectedPassword = PasswordDomainUser;
            string actualPassword = await _sgStore.GetValueAsync(ContextSafeguard, KeyDomain);
            actualPassword.Should().BeEquivalentTo(expectedPassword);
        }

        [Fact]
        public async void GetValueAsyncAPIKey()
        {
            string expectedPassword = PasswordAssetUser;
            string actualPassword = await _sgStore.GetValueAsync(ContextSafeguard, KeyApiKey);
            actualPassword.Should().BeEquivalentTo(expectedPassword);
        }

        [Fact]
        public async void GetCredentialAsyncAsset()
        {
            Credential expectedCredential = new Credential
            {
                Username = KeyAsset.Split('@')[0],
                Password = PasswordAssetUser
            };
            Credential actualCredential = await _sgStore.GetCredentialsAsync(ContextSafeguard, KeyAsset);
            actualCredential.Should().BeEquivalentTo(expectedCredential);
        }

        [Fact]
        public async void GetCredentialAsyncDomain()
        {
            Credential expectedCredential = new Credential
            {
                Username = KeyDomain.Split('\\')[1],
                Password = PasswordDomainUser
            };
            Credential actualCredential = await _sgStore.GetCredentialsAsync(ContextSafeguard, KeyDomain);
            actualCredential.Should().BeEquivalentTo(expectedCredential);
        }

        [Fact]
        public async void GetCredentialAsyncAPIKey()
        {
            Credential expectedCredential = new Credential
            {
                Username = KeyAsset.Split('@')[0],
                Password = PasswordAssetUser
            };
            Credential actualCredential = await _sgStore.GetCredentialsAsync(ContextSafeguard, KeyApiKey);
            actualCredential.Should().BeEquivalentTo(expectedCredential);
        }


        /*
        [Fact]
        public async void GetValueAsyncWIthCanFailTrueHasATenPercentChanceOfFailure()
        {
            double expectedFailRate = 0.1;
            double actualFailRate = await GetFailureRateAsync(
                () => subject.GetValueAsync(ContextCanFailTrue, string.Empty), 1000);
            Assert.Equal(expectedFailRate, actualFailRate);
        }

        [Fact]
        public async void GetCredentialsAsyncWithCanFailFalseReturnsCredentialWithGivenKeyAsUserNameAndARandomPassword()
        {
            var credential = await subject.GetCredentialsAsync(ContextCanFailFalse, DefaultKey);
            credential.Username.Should().BeEquivalentTo(DefaultKey);
            Assert.True(IsPasswordValid(credential.Password));
        }

        [Fact]
        public async void GetCredentialsAsyncWithCanFailTrueHasATenPercentChanceOfFailure()
        {
            double expectedFailRate = 0.1;
            double actualFailRate = await GetFailureRateAsync(
                () => subject.GetCredentialsAsync(ContextCanFailTrue, DefaultKey), 1000);
            Assert.Equal(expectedFailRate, actualFailRate);
        }

        [Fact]
        public async void CreateValueAsyncThrowsSecureStoreExceptionWithReadOnlyMessage()
        {
            var ex = await Assert.ThrowsAsync<SecureStoreException>(
                () => subject.CreateValueAsync(ContextCanFailFalse, DefaultKey, string.Empty));
            ex.Message.Should().BeEquivalentTo(ReadOnly);
        }

        [Fact]
        public async void CreateCredentialsAsyncThrowsSecureStoreExceptionWithReadOnlyMessage()
        {
            var ex = await Assert.ThrowsAsync<SecureStoreException>(
                () => subject.CreateCredentialsAsync(ContextCanFailFalse, DefaultKey, new Credential()));
            ex.Message.Should().BeEquivalentTo(ReadOnly);
        }

        [Fact]
        public async void UpdateValueAsyncThrowsSecureStoreExceptionWithReadOnlyMessage()
        {
            var ex = await Assert.ThrowsAsync<SecureStoreException>(
                () => subject.UpdateValueAsync(ContextCanFailFalse, DefaultKey, DefaultPasswordKey, string.Empty));
            ex.Message.Should().BeEquivalentTo(ReadOnly);
        }

        [Fact]
        public async void UpdateCredentialsAsyncThrowsSecureStoreExceptionWithReadOnlyMessage()
        {
            var ex = await Assert.ThrowsAsync<SecureStoreException>(
                () => subject.UpdateCredentialsAsync(ContextCanFailFalse, DefaultKey, DefaultPasswordKey, new Credential()));
            ex.Message.Should().BeEquivalentTo(ReadOnly);
        }

        [Fact]
        public async void RemoveValueAsyncThrowsSecureStoreExceptionWithReadOnlyMessage()
        {
            var ex = await Assert.ThrowsAsync<SecureStoreException>(
                () => subject.RemoveValueAsync(ContextCanFailFalse, DefaultKey));
            ex.Message.Should().BeEquivalentTo(ReadOnly);
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length != PasswordLength)
            {
                return false;
            }

            int digitCount = 0, capitalCount = 0, specialCount = 0;
            foreach (char c in password)
            {
                if (SpecialChars.Contains(c))
                {
                    specialCount++;
                }
                else if (char.IsDigit(c))
                {
                    digitCount++;
                }
                else if (char.IsUpper(c))
                {
                    capitalCount++;
                }
            }

            return digitCount >= 2 && capitalCount >= 2 && specialCount >= 1;
        }

        private async Task<double> GetFailureRateAsync(Func<Task> func, int times)
        {
            int failCount = 0;
            for (int i = 0; i < times; i++)
            {
                try
                {
                    await func();
                }
                catch (SecureStoreException ex)
                {
                    ex.Message.Should().BeEquivalentTo(SecretNotFound);
                    failCount++;
                }
            }

            return Math.Round(failCount / (double)times, 1);
        }
        */

    }
}
