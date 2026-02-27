using System;
using System.Collections.Generic;

namespace cog1.DTO
{
    /// <summary>
    /// Data transfer object representing an integration connection.
    /// Mirrors the <c>integration_connections</c> database table.
    /// </summary>
    public class IntegrationConnectionDTO
    {
        /// <summary>
        /// Unique identifier for the integration connection.
        /// </summary>
        public int integrationConnectionId { get; set; }

        /// <summary>
        /// Connection type.
        /// </summary>
        public IntegrationConnectionType connectionType { get; set; }

        /// <summary>
        /// Human-readable description of the connection.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// Base URL to use for HTTP-based integrations (e.g. https://api.example.com).
        /// </summary>
        public string httpBaseUrl { get; set; }

        /// <summary>
        /// Optional HTTP headers (serialized) to include on requests.
        /// </summary>
        public List<ValuePairDTO> httpHeaders { get; set; }

        /// <summary>
        /// Whether to use TLS when connecting to the MQTT broker.
        /// </summary>
        public bool mqttUseTls { get; set; }

        /// <summary>
        /// Hostname or IP address of the MQTT broker, and port. If the port is
        /// specified, it must be in the format host:port. When the port is ommitted,
        /// the default MQTT port 1883 is assumed.
        /// </summary>
        public string mqttHost { get; set; }

        /// <summary>
        /// Base topic to use when publishing/subscribing via MQTT.
        /// </summary>
        public string mqttBaseTopic { get; set; }

        /// <summary>
        /// PEM or other representation of the server certificate for MQTT TLS (optional).
        /// </summary>
        public string mqttServerCertificate { get; set; }

        /// <summary>
        /// PEM or other representation of the client certificate for MQTT TLS (optional).
        /// </summary>
        public string mqttClientCertificate { get; set; }

        /// <summary>
        /// Username for authenticating to the integration endpoint (if required).
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Password for authenticating to the integration endpoint (if required).
        /// </summary>
        public string password { get; set; }
    }
}
