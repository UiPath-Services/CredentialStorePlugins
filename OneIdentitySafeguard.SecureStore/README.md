# Introduction 
The SafeguardSecureStore plugin is created to integrate UiPath with One Identity Safeguard for Privileged Passwords (SPP). This is a read-only plugin implementing the following use-cases:
* Get Robot credentials from SPP by api-key or by AccountName@[AssetName|AssetNetworkAddress|DomainName]
* Get Asset credentials from SPP by api-key or by AccountName@[AssetName|AssetNetworkAddress|DomainName]
The plugin is based on OneIdentity.SafeguardDotNet 6.8.2

# Configuration
The integration is based on the Application to Application (A2A) credential retrieval functionality of SPP.

## SPP Configuration
Configure A2A according to the [SPP Administration guide](https://support.oneidentity.com/technical-documents/one-identity-safeguard-for-privileged-passwords/administration-guide):
* Create a certificate user in SPP (per each registered UiPath Credential Store) and upload the CA of the user's certificate to __Settings | Certificate | Trusted Certificates__
* [Enable the A2A service](https://support.oneidentity.com/technical-documents/one-identity-safeguard-for-privileged-passwords/administration-guide/77#TOPIC-1668512)
* [Register the UiPath Credential Store as an application in SPP](https://support.oneidentity.com/technical-documents/one-identity-safeguard-for-privileged-passwords/administration-guide/98#TOPIC-1668624). Each UiPath Credential Store is considered as a registered application with its own certificate user and there can be multiple Credential Stores registered.
  * Enable the __Visible to certificate user__ setting in case you want to get credentials by AccountName@[AssetName|AssetNetworkAddress|DomainName]

## Orchestrator Credential store configuration
TODO

## Orchestrator and Machine certificate configuration
Make sure the A2A user's PFX certificate is imported into the local Windows certificate store, either to the Computer store or the store of the user account running the application.

# Usage
## Get Robot credentials from SPP by api-key
TODO
## Get Robot credentials from SPP by AccountName@[AssetName|AssetNetworkAddress|DomainName]
TODO
## Get Asset credentials from SPP by api-key
TODO
## Get Asset credentials from SPP by AccountName@[AssetName|AssetNetworkAddress|DomainName]
TODO
