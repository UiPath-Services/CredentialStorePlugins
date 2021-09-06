namespace UiPath.Orchestrator.Extensions.SecureStores.OneIdentitySafeguard
{
    public sealed class OneIdentitySafeguardContext
    {
        public string Hostname { get; set; }
        public string Thumbprint { get; set; }
        public bool IgnoreSSL { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Hostname != null ? Hostname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Thumbprint != null ? Thumbprint.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IgnoreSSL.GetHashCode();
                return hashCode;
            }
        }
    }
}