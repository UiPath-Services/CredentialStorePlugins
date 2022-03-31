# Introduction 
The SafeguardSecureStore plugin is created to integrate UiPath with One Identity Safeguard for Privileged Passwords (SPP). This is a read-only plugin implementing the following use-cases:
* Get Robot credentials from SPP by using an External Name formatted as sgkey:{a2a-api-key-from-spp} or by DomainName\AccountName
* Get Asset credentials from SPP by using an External Name formatted as sgkey:{a2a-api-key-from-spp} or by AccountName@[AssetName|AssetNetworkAddress|DomainName]

The integration is based on the Application to Application (A2A) credential retrieval functionality of SPP. The Orchestrator acts a credential broker for both unattended or attended scenarios, the robots are not connected to SPP but to the Orchestrator.
Access request workflow based password release is currently not supported, open an issue on GitHub or contact One Identity in case of having such interest.

# Releases
See the [Releases](https://github.com/UiPath-Services/CredentialStorePlugins/releases?q=OneIdentity.SafeguardSecureStore&expanded=true) for a plugin working with your Orchestrator and SPP versions.

# Pre-requisites
* The Orchestrator has access to SPP on port 443.
*	A PFX certificate that will be used by the Orchestrator to connect to Safeguard.
*	The above certificate must be validated by one of the certificates already available on Safeguard (Settings | Certificates | Trusted Certificates) or a new certificate must be created for this purpose too.
*	In case you wish to enable SSL certificate validation on the side of the Orchestrator too, the signing CA of the Safeguard SSL certificate is required too (available at Settings | Certificates | SSL certificates) and Safeguard must be reachable from the Orchestrator via one of the IP addresses or DNS names available in the CN or SAN fields of the Safeguard SSL certificate.
*	Customer personnel with administrative rights on Safeguard in order to configure the user and the application registration settings of the Orchestrator (see the configuration steps below).
*	Customer personnel with administrative rights to manage the certificates on the Orchestrator machine (Local Computer certificate store, see the configuration steps below)
*	The plugin downloaded from Releases.


# Configuration

## SPP Configuration
Configure A2A according to the [SPP Administration guide](https://support.oneidentity.com/technical-documents/one-identity-safeguard-for-privileged-passwords/administration-guide):
* Create a certificate user in SPP (per each registered UiPath Credential Store) and upload the CA of the user's certificate to __Settings | Certificate | Trusted Certificates__
* [Enable the A2A service](https://support.oneidentity.com/technical-documents/one-identity-safeguard-for-privileged-passwords/administration-guide/77#TOPIC-1668512)
* [Register the UiPath Credential Store as an application in SPP](https://support.oneidentity.com/technical-documents/one-identity-safeguard-for-privileged-passwords/administration-guide/98#TOPIC-1668624). Each UiPath Credential Store is considered as a registered application with its own certificate user and there can be multiple Credential Stores registered.
  * Enable the __Visible to certificate user__ setting in case you want to get credentials by AccountName@[AssetName|AssetNetworkAddress|DomainName]

## Orchestrator Credential store configuration
* Copy the UiPath.Orchestrator.SafeguardSecureStore.dll into the Orchestrator\plugins folder as well as the corresponding dependencies. The requires dependencies should be included into the release zip bundle (netstandard2.0).
* Enable the SafeguardSecureStore plugin as instructed by the UiPath documentation. Open the Orchestrator\UiPath.Orchestrator.dll.config file and add the following line to `<appSettings>`: `<add key="Plugins.SecureStores" value="UiPath.Orchestrator.SafeguardSecureStore.dll"/>`
* Navigate to __Tenant | Credential Stores__
* Create a new Credential Store:
  * Type: One Identity Safeguard
  * Name: give a meaningful name
  * Safeguard Appliance: enter the IP address or the hostname of the Safeguard SPP appliance
  * Certificate thumbprint used to connect to Safeguard: Enter the thumbprint of the certificate that is stored in the Windows certificate store and will be used to connect to Safeguard.
  * Do not validate the SSL certificate of Safeguard (IgnoreSSL):
    * Set to true in case you don't want the Orchestrator to validate the SSL certificate of Safeguard.  
    * Set to false in case you want the Orchestrator to validate the SSL certificate of Safeguard. If so:
      * Make sure the signing CA of the Safeguard SSL certificate (Settings || Certificates || SSL Certificates) is imported into the Intermediate/Trusted Certification Authorities of the Computer Certificate Store of Windows
      * Ensure that Safeguard is reachable via one of the IP addresses or DNS names in the CN or SAN fields of the SSL certificate.
  * Debug Logging: If set to true, the plugin writes debug execution logs into the Application Eventlog container with source: UiPath. Leave it on false unless troubleshooting requires further logs than a thrown exception (also visible in Application Eventlog).

## Orchestrator certificate configuration
Make sure the A2A user's PFX certificate is imported into the Computer Store of the local Windows certificate store of the machine where the Orchestrator runs.

Make sure that the account running the Orchestrator has access to the certificate (bey default ApplicationPoolIdentity):
* The private key of the Safeguard A2A user should be marked as exportable in the Certificate store. Open CMD as administrator and enter the following command: `certutil -store MY`. Locate the certificate in the dump. In case there is no line with _Private key is NOT exportable_ in the output, the private key is exportable. If the private key is not exportable, import the certificate again with exportable private key.
* Open the MMC Certificate Store of the Local Computer, Navigate to personal certificates. 
* Find the certificate of the Safeguard A2A user.
* Right-click the certificate of the Safeguard A2A user and click __All Tasks > Manage Private Keys...__ and click the __Add__ button in the permission window.
* Select __Location: Computer__ instead of the directory and enter __IIS AppPool\UiPath Orchestrator (or the Application Pool UiPath is ran by)__ into the field of object names to select. Click OK
* Enable __Read__ permissions for the Application Pool Identity

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
* Obtain the A2A API key of the asset account from your Safeguard administrator.
* Navigate to the __{Desired Workspace} | Assets__ and create a new Asset
* Set the Asset name and Select the previously created Safeguard Credential Store
* In case the asset refers to a globally unique account, enable _Global Value_ and add the obtained key with the sgkey: prefix to the _External Name_ field: `sgkey:{your-a2a-api-key here}`
* In case the asset varies per user/machine then click the _+_ button and define the desired user/machine pair, then add the obtained key with the sgkey: prefix to the _External Name_ field: `sgkey:{your-a2a-api-key here}`
* Whenever the `Get Credentials` activity is called for this asset by a robot, it will retrieve the asset password from Safeguard.

## Get Asset credentials from SPP by AccountName@[AssetName|AssetNetworkAddress|DomainName]
* Navigate to the __{Desired Workspace} | Assets__ and create a new Asset
* Set the Asset name and Select the previously created Safeguard Credential Store
* In case the asset refers to a globally unique account, enable __Global Value__ and set the __External Name__ with one of the following templates (referring to the attributes of the Account stored in Safeguard):
  * AccountName@AssetName 
  * AccountName@AssetNetworkAddress 
  * AccountName@DomainName
  * DomainName\AccountName
* In case the asset varies per user/machine then click the __+__ button and define the desired user/machine pair, then set the __External Name__ with one of the following templates (referring to the attributes of the Account stored in Safeguard):
  * AccountName@AssetName 
  * AccountName@AssetNetworkAddress 
  * AccountName@DomainName
  * DomainName\AccountName
* Whenever the `Get Credentials` activity is called for this asset by a robot, it will retrieve the asset password from Safeguard.

# Troubleshooting
The following errors may show up in the Application Eventlog.
## Authorization is required for this request
Possible causes:
* The Certificate of the A2A user is invalid or expired
* The __Visible to Certificate User__ option is not enabled in SPP although the credential request is made by AccountName@[AssetName|AssetNetworkAddress|DomainName]
* Wrong asset value is configured, the requested account is not listed in the A2A registration
* Invalid sgkey given

## Unable to connect to web service https://{safeguard-address}/service/core/v3, Error: An error occurred while sending the request. The read operation failed, see inner exception.
* Check network connectivity from the Orchestrator to {safeguard-address}:443
* Make sure the Application Identity running the Orchestrator has access to the certificate store where the A2A certificate is stored. Apply one of the following options:
  * Use a dedicated account to run the Orchestrator.
  * Grant access to the certificate stored in the certificate store for the Application Pool Identitity, see above at 'Orchestrator certificate configuration'
