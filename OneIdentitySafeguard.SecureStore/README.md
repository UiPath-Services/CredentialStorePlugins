# Introduction 
The SafeguardSecureStore plugin is created to integrate UiPath with One Identity Safeguard for Privileged Passwords (SPP). This is a read-only plugin implementing the following use-cases:
* Get Robot credentials from SPP by sgkey:{a2a-api-key-from-spp} or by DomainName\AccountName
* Get Asset credentials from SPP by sgkey:{a2a-api-key-from-spp} or by AccountName@[AssetName|AssetNetworkAddress|DomainName]

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
* Copy the UiPath.Orchestrator.SafeguardSecureStore.dll 6.8.2 into the Orchestrator\plugins folder. Also cope the following DLL dependencies into the same folder:
  * Microsoft.Extensions.Caching.Abstractions.dll
  * Microsoft.Extensions.Caching.Memory.dll minversion: 5.0.0.
  * Microsoft.Extensions.DependencyInjection.Abstractions.dll
  * Microsoft.Extensions.Logging.Abstractions.dll
  * Microsoft.Extensions.Options.dll
  * Microsoft.Extensions.Primitives.dll
  * RestSharp.dll minversion: 106.11.7
  * Serilog.dll minversion: 2.10.0
  * Serilog.Sinks.EventLog.dll minversion 3.1.0
* Enable the SafeguardSecureStore plugin as instructed by the UiPath guide. Open the Orchestrator\UiPath.Orchestrator.dll.config file and add the following line to `<appSettings>`: `<add key="Plugins.SecureStores" value="UiPath.Orchestrator.SafeguardSecureStore.dll"/>`
* Navigate to __Tenant | Credential Stores__
* Create a new Credential Store:
  * Type: One Identity Safeguard
  * Name: give a meaningful name
  * Safeguard Appliance: enter the IP address or the hostname of the Safeguard SPP appliance
  * Certificate thumbprint used to connect to Safeguard: Enter the thumbprint of the certificate that is stored in the Windows certificate store and will be used to connect to Safeguard (enter it in a format with capital letters).
  * Ignore SSL:
    * Set to true in case you don't want the Orchestrator to validate the SSL certificate of Safeguard.  
    * Set to false in case you want the Orchestrator to validate the SSL certificate of Safeguard. If so:
      * TODO
  * Debug Logging: If the to true, the plugin writes debug execution logs into the Application Eventlog container. Leave it on false unless troubleshooting requires further logs than a thrown exception.

## Orchestrator certificate configuration
Make sure the A2A user's PFX certificate is imported into the Computer Store of the local Windows certificate store of the machine where the Orchestrator runs.

In case the Orhcestrator is executed by a local ApplicationPoolIdentity instead of a domain account, make sure that the ApplicationPoolIdentity has access to the certificate:
* TODO

# Usage
## Get Robot credentials from SPP by DomainName\AccountName
* Navigate to __Tenant | Users__
* Click the 3 dots on the user which you would like to use the Safeguard Credential Store. Click Edit.
* Navigate to the option Unattended Robot, and set the Credential Store to the one just created.
* Enter the domain\username of the account which the robot will checkout the password for from Safeguard.
## Get Robot credentials from SPP by api-key
* Obtain the A2A API key of the robot account from your Safeguard administrator.
* Navigate to __Tenant | Users__
* Click the 3 dots on the user which you would like to use the Safeguard Credential Store. Click Edit.
* Navigate to the option Unattended Robot, and set the Credential Store to the one just created.
* Enter the domain\username of the robot account.
* Enter the A2A API key into the External Name field with the 'sgkey:' prefix.

## Get Asset credentials from SPP by api-key
TODO
## Get Asset credentials from SPP by AccountName@[AssetName|AssetNetworkAddress|DomainName]
TODO

# Troubleshooting
The following errors may show up in the Application Eventlog.
## Authorization is required for this request
Possible causes:
* The Certificate of the A2A user is invalid or expired
* 'Visible to Certificate User' is not enabled although the credential request is made by AccountName@[AssetName|AssetNetworkAddress|DomainName]
* Wrong asset value is configured, the requested account is not listed in the A2A registration
* Invalid sgkey given

## Unable to connect to web service https://{safeguard-address}/service/core/v3, Error: An error occurred while sending the request. The read operation failed, see inner exception.:
* Check network connectivity from the Orchestrator to {safeguard-address}:443
* Make sure the Application Identity running the Orchestrator has access to the certificate store where the A2A certificate is stored. Apply one of the following options:
  * Use a dedicated account to run the Orchestrator.
  * Grant access to the certificate stored in the certificate store for the Application Pool Identitity, see above at 'Orchestrator certificate configuration'
