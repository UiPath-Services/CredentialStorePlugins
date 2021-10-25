namespace UiPath.Orchestrator.Extensions.SecureStores.Safeguard
{
    public class SafeguardContext
    {
        public string SafeguardAppliance { get; set; }
        public string SafeguardCertThumbprint { get; set; }
        public bool IgnoreSSL { get; set; }
        public bool DebugLogging { get; set; }
    }
}