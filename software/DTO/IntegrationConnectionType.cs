namespace cog1.DTO
{
    /// <summary>
    /// Types of integration connections supported by the system.
    /// Replaces the previous <c>OutboundIntegrationType</c> enum name.
    /// </summary>
    public enum IntegrationConnectionType
    {
        Unknown = 0,
        MQTT = 1,
        HTTPPost = 2,
    }
}
