using System.Collections.Generic;

namespace cog1.DTO
{
    /// <summary>
    /// This entity describes an outbound integration. 
    /// </summary>
    public class OutboundIntegrationDTO
    {
        /// <summary>
        /// Unique identifier of the outbound integration. This is automatically
        /// generated when the integration is created.
        /// </summary>
        public int integrationId { get; set; }

        /// <summary>
        /// Human-readable description of the integration.
        /// Mirrors the <c>description</c> column in the DB.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Reference to the integration connection to use for this outbound integration.
        /// Links to <c>IntegrationConnectionDTO.integrationConnectionId</c>.
        /// </summary>
        public int integrationConnectionId { get; set; }

        /// <summary>
        /// HTTP URL to post to (when using HTTP integrations).
        /// </summary>
        public string httpUrl { get; set; }

        /// <summary>
        /// MQTT topic to publish to (when using MQTT integrations).
        /// </summary>
        public string mqttTopic { get; set; }

        /// <summary>
        /// Interval in seconds between automatic sends for this integration.
        /// </summary>
        public int sendIntervalSeconds { get; set; }

        /// <summary>
        /// List of variable IDs whose changes trigger a send. 
        /// </summary>
        public List<int> variableChangeList { get; set; }

        /// <summary>
        /// Number of minutes to buffer reports when the gateway is offline. 
        /// Buffered reports older than this will be purged automatically.
        /// </summary>
        public int reportBufferingMinutes { get; set; }

        /// <summary>
        /// Template used to render the report payload.
        /// </summary>
        public string reportTemplate { get; set; }
    }
}