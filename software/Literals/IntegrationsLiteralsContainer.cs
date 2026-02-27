namespace cog1.Literals
{
    public class IntegrationsLiteralsContainer : BaseLiteralsContainer
    {
        public IntegrationsLiteralsContainer() : base() { }
        public IntegrationsLiteralsContainer(string localeCode) : base(localeCode) { }

        public virtual string Connection { get => new IntegrationsLiterals.Connection().ExtractLiteral(LocaleCode); set { } }
        public virtual string Connections { get => new IntegrationsLiterals.Connections().ExtractLiteral(LocaleCode); set { } }
        public virtual string Integrations { get => new IntegrationsLiterals.Integrations().ExtractLiteral(LocaleCode); set { } }
        public virtual string NewConnection { get => new IntegrationsLiterals.NewConnection().ExtractLiteral(LocaleCode); set { } }
        public virtual string EditConnection { get => new IntegrationsLiterals.EditConnection().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteConnection { get => new IntegrationsLiterals.DeleteConnection().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteConnectionConfirmation { get => new IntegrationsLiterals.DeleteConnectionConfirmation().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConnectionId { get => new IntegrationsLiterals.ConnectionId().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConnectionType { get => new IntegrationsLiterals.ConnectionType().ExtractLiteral(LocaleCode); set { } }
        public virtual string MqttHost { get => new IntegrationsLiterals.MqttHost().ExtractLiteral(LocaleCode); set { } }
        public virtual string HttpBaseUrl { get => new IntegrationsLiterals.HttpBaseUrl().ExtractLiteral(LocaleCode); set { } }
        public virtual string MqttBaseTopic { get => new IntegrationsLiterals.MqttBaseTopic().ExtractLiteral(LocaleCode); set { } }
        public virtual string HttpHeaderName { get => new IntegrationsLiterals.HttpHeaderName().ExtractLiteral(LocaleCode); set { } }
        public virtual string HttpHeaderValue { get => new IntegrationsLiterals.HttpHeaderValue().ExtractLiteral(LocaleCode); set { } }
        public virtual string AddHttpHeader { get => new IntegrationsLiterals.AddHttpHeader().ExtractLiteral(LocaleCode); set { } }
        public virtual string Credentials { get => new IntegrationsLiterals.Credentials().ExtractLiteral(LocaleCode); set { } }
        public virtual string HttpHeaders { get => new IntegrationsLiterals.HttpHeaders().ExtractLiteral(LocaleCode); set { } }
        public virtual string MqttUseTls { get => new IntegrationsLiterals.MqttUseTls().ExtractLiteral(LocaleCode); set { } }
        public virtual string MqttServerCertificate { get => new IntegrationsLiterals.MqttServerCertificate().ExtractLiteral(LocaleCode); set { } }
        public virtual string MqttClientCertificate { get => new IntegrationsLiterals.MqttClientCertificate().ExtractLiteral(LocaleCode); set { } }
        public virtual string NoConnectionsToDisplay { get => new IntegrationsLiterals.NoConnectionsToDisplay().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConnectionCreated { get => new IntegrationsLiterals.ConnectionCreated().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConnectionUpdated { get => new IntegrationsLiterals.ConnectionUpdated().ExtractLiteral(LocaleCode); set { } }
        public virtual string ConnectionDeleted { get => new IntegrationsLiterals.ConnectionDeleted().ExtractLiteral(LocaleCode); set { } }
        public virtual string OutboundIntegration { get => new IntegrationsLiterals.OutboundIntegration().ExtractLiteral(LocaleCode); set { } }
        public virtual string OutboundIntegrations { get => new IntegrationsLiterals.OutboundIntegrations().ExtractLiteral(LocaleCode); set { } }
        public virtual string NewOutboundIntegration { get => new IntegrationsLiterals.NewOutboundIntegration().ExtractLiteral(LocaleCode); set { } }
        public virtual string EditOutboundIntegration { get => new IntegrationsLiterals.EditOutboundIntegration().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteOutboundIntegration { get => new IntegrationsLiterals.DeleteOutboundIntegration().ExtractLiteral(LocaleCode); set { } }
        public virtual string DeleteOutboundIntegrationConfirmation { get => new IntegrationsLiterals.DeleteOutboundIntegrationConfirmation().ExtractLiteral(LocaleCode); set { } }
        public virtual string OutboundIntegrationDeleted { get => new IntegrationsLiterals.OutboundIntegrationDeleted().ExtractLiteral(LocaleCode); set { } }
        public virtual string NoOutboundIntegrationsToDisplay { get => new IntegrationsLiterals.NoOutboundIntegrationsToDisplay().ExtractLiteral(LocaleCode); set { } }
        public virtual string MqttSubTopic { get => new IntegrationsLiterals.MqttSubTopic().ExtractLiteral(LocaleCode); set { } }
        public virtual string HttpSubUrl { get => new IntegrationsLiterals.HttpSubUrl().ExtractLiteral(LocaleCode); set { } }
        public virtual string Template { get => new IntegrationsLiterals.Template().ExtractLiteral(LocaleCode); set { } }
        public virtual string SendIntervalSeconds { get => new IntegrationsLiterals.SendIntervalSeconds().ExtractLiteral(LocaleCode); set { } }
        public virtual string ReportBufferingMinutes { get => new IntegrationsLiterals.ReportBufferingMinutes().ExtractLiteral(LocaleCode); set { } }
        public virtual string OutboundIntegrationCreated { get => new IntegrationsLiterals.OutboundIntegrationCreated().ExtractLiteral(LocaleCode); set { } }
        public virtual string OutboundIntegrationUpdated { get => new IntegrationsLiterals.OutboundIntegrationUpdated().ExtractLiteral(LocaleCode); set { } }
    }
}
