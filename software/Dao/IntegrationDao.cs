using cog1.Business;
using cog1.DTO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace cog1.Dao
{
    public class IntegrationDao : DaoBase
    {
        public IntegrationDao(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        private IntegrationConnectionDTO MakeIntegrationConnection(DataRow r)
        {
            return new IntegrationConnectionDTO()
            {
                integrationConnectionId = (int)r.Field<long>("integration_connection_id"),
                connectionType = (IntegrationConnectionType)r.Field<long>("connection_type"),
                description = r.Field<string>("description"),
                httpBaseUrl = r.Field<string>("http_base_url"),
                httpHeaders = JsonConvert.DeserializeObject<List<ValuePairDTO>>(r.Field<string>("http_headers")),
                mqttHost = r.Field<string>("mqtt_host"),
                mqttBaseTopic = r.Field<string>("mqtt_base_topic"),
                mqttServerCertificate = r.Field<string>("mqtt_server_certificate"),
                mqttClientCertificate = r.Field<string>("mqtt_client_certificate"),
                userName = r.Field<string>("user_name"),
                password = r.Field<string>("password"),
            };
        }

        public List<IntegrationConnectionDTO> EnumerateIntegrationConnections()
        {
            return Context.Db.GetDataTable("select * from integration_connections")
                .AsEnumerable()
                .Select(row => MakeIntegrationConnection(row))
                .ToList();
        }

        public IntegrationConnectionDTO GetIntegrationConnection(int id)
        {
            var t = Context.Db.GetDataTable("select * from integration_connections where integration_connection_id = @id", new() { { "@id", id } });
            if (t.Rows.Count == 0) return null;
            return MakeIntegrationConnection(t.Rows[0]);
        }

        public void CreateIntegrationConnection(IntegrationConnectionDTO dto)
        {
            if (dto.integrationConnectionId == 0)
            {
                dto.integrationConnectionId = (int)((Context.Db.GetLong("select coalesce(max(integration_connection_id),0) from integration_connections") ?? 0) + 1);
            }
            Context.Db.Execute(
                "insert into integration_connections (integration_connection_id, connection_type, description, http_base_url, http_headers, mqtt_host, mqtt_base_topic, mqtt_server_certificate, mqtt_client_certificate, user_name, password) " +
                "values (@integration_connection_id, @connection_type, @description, @http_base_url, @http_headers, @mqtt_host, @mqtt_base_topic, @mqtt_server_certificate, @mqtt_client_certificate, @user_name, @password)",
                new()
                {
                    { "@integration_connection_id", dto.integrationConnectionId },
                    { "@connection_type", dto.connectionType },
                    { "@description", string.IsNullOrWhiteSpace(dto.description) ? DBNull.Value : dto.description.Trim() },
                    { "@http_base_url", string.IsNullOrWhiteSpace(dto.httpBaseUrl) ? DBNull.Value : dto.httpBaseUrl.Trim() },
                    { "@http_headers", JsonConvert.SerializeObject(dto.httpHeaders) },
                    { "@mqtt_host", string.IsNullOrWhiteSpace(dto.mqttHost) ? DBNull.Value : dto.mqttHost.Trim() },
                    { "@mqtt_base_topic", string.IsNullOrWhiteSpace(dto.mqttBaseTopic) ? DBNull.Value : dto.mqttBaseTopic.Trim() },
                    { "@mqtt_server_certificate", string.IsNullOrWhiteSpace(dto.mqttServerCertificate) ? DBNull.Value : dto.mqttServerCertificate.Trim() },
                    { "@mqtt_client_certificate", string.IsNullOrWhiteSpace(dto.mqttClientCertificate) ? DBNull.Value : dto.mqttClientCertificate.Trim() },
                    { "@user_name", string.IsNullOrWhiteSpace(dto.userName) ? DBNull.Value : dto.userName.Trim() },
                    { "@password", string.IsNullOrWhiteSpace(dto.password) ? DBNull.Value : dto.password.Trim() },
                });
        }

        public void EditIntegrationConnection(IntegrationConnectionDTO dto)
        {
            Context.Db.Execute(
                "update integration_connections set connection_type = @connection_type, description = @description, http_base_url = @http_base_url, http_headers = @http_headers, mqtt_host = @mqtt_host, mqtt_base_topic = @mqtt_base_topic, mqtt_server_certificate = @mqtt_server_certificate, mqtt_client_certificate = @mqtt_client_certificate, user_name = @user_name, password = @password where integration_connection_id = @integration_connection_id",
                new()
                {
                    { "@connection_type", dto.connectionType },
                    { "@description", string.IsNullOrWhiteSpace(dto.description) ? DBNull.Value : dto.description.Trim() },
                    { "@http_base_url", string.IsNullOrWhiteSpace(dto.httpBaseUrl) ? DBNull.Value : dto.httpBaseUrl.Trim() },
                    { "@http_headers", JsonConvert.SerializeObject(dto.httpHeaders) },
                    { "@mqtt_host", string.IsNullOrWhiteSpace(dto.mqttHost) ? DBNull.Value : dto.mqttHost.Trim() },
                    { "@mqtt_base_topic", string.IsNullOrWhiteSpace(dto.mqttBaseTopic) ? DBNull.Value : dto.mqttBaseTopic.Trim() },
                    { "@mqtt_server_certificate", string.IsNullOrWhiteSpace(dto.mqttServerCertificate) ? DBNull.Value : dto.mqttServerCertificate.Trim() },
                    { "@mqtt_client_certificate", string.IsNullOrWhiteSpace(dto.mqttClientCertificate) ? DBNull.Value : dto.mqttClientCertificate.Trim() },
                    { "@user_name", string.IsNullOrWhiteSpace(dto.userName) ? DBNull.Value : dto.userName.Trim() },
                    { "@password", string.IsNullOrWhiteSpace(dto.password) ? DBNull.Value : dto.password.Trim() },
                    { "@integration_connection_id", dto.integrationConnectionId },
                });
        }

        public bool HasOutboundIntegrations(int integrationConnectionId)
        {
            return Context.Db.GetLong("select 1 from outbound_integrations where integration_connection_id = @id", new() { { "@id", integrationConnectionId } }) != null;
        }

        public void DeleteIntegrationConnection(int integrationConnectionId)
        {
            Context.Db.Execute("delete from integration_connections where integration_connection_id = @id", new() { { "@id", integrationConnectionId } });
        }

        private OutboundIntegrationDTO MakeOutboundIntegration(DataRow r)
        {
            return new OutboundIntegrationDTO()
            {
                integrationId = (int)r.Field<long>("integration_id"),
                integrationConnectionId = (int)r.Field<long>("integration_connection_id"),
                description = r.Field<string>("description"),
                httpUrl = r.Field<string>("http_url"),
                mqttTopic = r.Field<string>("mqtt_topic"),
                sendIntervalSeconds = (int)r.Field<long>("send_interval_seconds"),
                variableChangeList = JsonConvert.DeserializeObject<List<int>>(r.Field<string>("variable_change_list")),
                reportBufferingMinutes = (int)r.Field<long>("report_buffering_minutes"),
                reportTemplate = r.Field<string>("report_template"),
            };
        }

        public List<OutboundIntegrationDTO> EnumerateOutboundIntegrations()
        {
            return Context.Db.GetDataTable("select * from outbound_integrations")
                .AsEnumerable()
                .Select(row => MakeOutboundIntegration(row))
                .ToList();
        }

        public OutboundIntegrationDTO GetOutboundIntegration(int id)
        {
            var t = Context.Db.GetDataTable("select * from outbound_integrations where integration_id = @id", new() { { "@id", id } });
            if (t.Rows.Count == 0) return null;
            return MakeOutboundIntegration(t.Rows[0]);
        }

        public int CreateOutboundIntegration(OutboundIntegrationDTO dto)
        {
            dto.integrationId = (int)((Context.Db.GetLong("select coalesce(max(integration_id),0) from outbound_integrations") ?? 0) + 1);
            Context.Db.Execute(
                "insert into outbound_integrations (integration_id, integration_connection_id, description, http_url, mqtt_topic, send_interval_seconds, variable_change_list, report_buffering_minutes, report_template) " +
                "values (@integration_id, @integration_connection_id, @description, @http_url, @mqtt_topic, @send_interval_seconds, @variable_change_list, @report_buffering_minutes, @report_template)",
                new()
                {
                    { "@integration_id", dto.integrationId },
                    { "@integration_connection_id", dto.integrationConnectionId },
                    { "@description", string.IsNullOrWhiteSpace(dto.description) ? DBNull.Value : dto.description.Trim() },
                    { "@http_url", string.IsNullOrWhiteSpace(dto.httpUrl) ? DBNull.Value : dto.httpUrl.Trim() },
                    { "@mqtt_topic", string.IsNullOrWhiteSpace(dto.mqttTopic) ? DBNull.Value : dto.mqttTopic.Trim() },
                    { "@send_interval_seconds", dto.sendIntervalSeconds },
                    { "@variable_change_list", JsonConvert.SerializeObject(dto.variableChangeList) },
                    { "@report_buffering_minutes", dto.reportBufferingMinutes },
                    { "@report_template", string.IsNullOrWhiteSpace(dto.reportTemplate) ? DBNull.Value : dto.reportTemplate.Trim() },
                });
            return dto.integrationId;
        }

        public void EditOutboundIntegration(OutboundIntegrationDTO dto)
        {
            Context.Db.Execute(
                "update outbound_integrations set integration_connection_id = @integration_connection_id, description = @description, http_url = @http_url, mqtt_topic = @mqtt_topic, send_interval_seconds = @send_interval_seconds, variable_change_list = @variable_change_list, report_buffering_minutes = @report_buffering_minutes, report_template = @report_template where integration_id = @integration_id",
                new()
                {
                    { "@integration_connection_id", dto.integrationConnectionId },
                    { "@description", string.IsNullOrWhiteSpace(dto.description) ? DBNull.Value : dto.description.Trim() },
                    { "@http_url", string.IsNullOrWhiteSpace(dto.httpUrl) ? DBNull.Value : dto.httpUrl.Trim() },
                    { "@mqtt_topic", string.IsNullOrWhiteSpace(dto.mqttTopic) ? DBNull.Value : dto.mqttTopic.Trim() },
                    { "@send_interval_seconds", dto.sendIntervalSeconds },
                    { "@variable_change_list", JsonConvert.SerializeObject(dto.variableChangeList) },
                    { "@report_buffering_minutes", dto.reportBufferingMinutes },
                    { "@report_template", string.IsNullOrWhiteSpace(dto.reportTemplate) ? DBNull.Value : dto.reportTemplate.Trim() },
                    { "@integration_id", dto.integrationId },
                });
        }

        public void DeleteOutboundIntegration(int integrationId)
        {
            Context.Db.Execute("delete from outbound_integrations where integration_id = @id", new() { { "@id", integrationId } });
        }
    }
}
