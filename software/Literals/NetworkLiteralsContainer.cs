namespace cog1.Literals
{
    public class NetworkLiteralsContainer : BaseLiteralsContainer
    {
        public NetworkLiteralsContainer() : base() { }
        public NetworkLiteralsContainer(string localeCode) : base(localeCode) { }
        public virtual string Network { get => new NetworkLiterals.Network().ExtractLiteral(LocaleCode); set { } }
        public virtual string Summary { get => new NetworkLiterals.Summary().ExtractLiteral(LocaleCode); set { } }
        public virtual string NetworkSummary { get => new NetworkLiterals.NetworkSummary().ExtractLiteral(LocaleCode); set { } }
        public virtual string Connection { get => new NetworkLiterals.Connection().ExtractLiteral(LocaleCode); set { } }
        public virtual string Status { get => new NetworkLiterals.Status().ExtractLiteral(LocaleCode); set { } }
        public virtual string Connected { get => new NetworkLiterals.Connected().ExtractLiteral(LocaleCode); set { } }
        public virtual string Disconnected { get => new NetworkLiterals.Disconnected().ExtractLiteral(LocaleCode); set { } }
        public virtual string IpMethod { get => new NetworkLiterals.IpMethod().ExtractLiteral(LocaleCode); set { } }
        public virtual string IpFixed { get => new NetworkLiterals.IpFixed().ExtractLiteral(LocaleCode); set { } }
        public virtual string IpAddress { get => new NetworkLiterals.IpAddress().ExtractLiteral(LocaleCode); set { } }
        public virtual string Gateway { get => new NetworkLiterals.Gateway().ExtractLiteral(LocaleCode); set { } }
        public virtual string Frequency { get => new NetworkLiterals.Frequency().ExtractLiteral(LocaleCode); set { } }
        public virtual string Speed { get => new NetworkLiterals.Speed().ExtractLiteral(LocaleCode); set { } }
        public virtual string FullDuplex { get => new NetworkLiterals.FullDuplex().ExtractLiteral(LocaleCode); set { } }
        public virtual string HalfDuplex { get => new NetworkLiterals.HalfDuplex().ExtractLiteral(LocaleCode); set { } }
        public virtual string MacAddress { get => new NetworkLiterals.MacAddress().ExtractLiteral(LocaleCode); set { } }
        public virtual string Connect { get => new NetworkLiterals.Connect().ExtractLiteral(LocaleCode); set { } }
        public virtual string Disconnect { get => new NetworkLiterals.Disconnect().ExtractLiteral(LocaleCode); set { } }
        public virtual string Forget { get => new NetworkLiterals.Forget().ExtractLiteral(LocaleCode); set { } }
        public virtual string Scanning { get => new NetworkLiterals.Scanning().ExtractLiteral(LocaleCode); set { } }
        public virtual string ScanningPleaseWait { get => new NetworkLiterals.ScanningPleaseWait().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConnectingPleaseWait { get => new NetworkLiterals.ConnectingPleaseWait().ExtractLiteral(LocaleCode); set { } }
        public virtual string DisconnectingPleaseWait { get => new NetworkLiterals.DisconnectingPleaseWait().ExtractLiteral(LocaleCode); set { } }
        public virtual string ForgettingPleaseWait { get => new NetworkLiterals.ForgettingPleaseWait().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConfiguringPleaseWait { get => new NetworkLiterals.ConfiguringPleaseWait().ExtractLiteral(LocaleCode); set { } }
        public virtual string IpConfiguration { get => new NetworkLiterals.IpConfiguration().ExtractLiteral(LocaleCode); set { } }
        public virtual string LinkConfiguration { get => new NetworkLiterals.LinkConfiguration().ExtractLiteral(LocaleCode); set { } }
        public virtual string WiFiNetworks { get => new NetworkLiterals.WiFiNetworks().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConfirmChanges { get => new NetworkLiterals.ConfirmChanges().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConfirmForget { get => new NetworkLiterals.ConfirmForget().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConfigurationAppliedSuccessfully { get => new NetworkLiterals.ConfigurationAppliedSuccessfully().ExtractLiteral(LocaleCode); set { } }

    }

}
