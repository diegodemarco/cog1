using cog1.DTO;
using cog1.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace cog1.Business
{
    /// <summary>
    /// Business logic for integration connections and outbound integrations.
    /// Uses IntegrationDao for all database access.
    /// </summary>
    public class IntegrationBusiness : BusinessBase
    {
        public IntegrationBusiness(Cog1Context context, ILogger logger) : base(context, logger)
        {
        }

        #region Outbound integration change subscriptions

        /// <summary>
        /// Represents a subscriber that wants to be notified when the outbound
        /// integration configuration changes (create, edit or delete).
        /// </summary>
        public class OutboundIntegrationChangeSubscription
        {
            /// <summary>
            /// Signalled whenever an outbound integration is created, edited or deleted.
            /// Subscribers can wait on this event to react promptly to changes.
            /// </summary>
            public AutoResetEvent ChangedEvent { get; } = new(false);
        }

        private static readonly List<OutboundIntegrationChangeSubscription> outboundChangeSubscriptions = new();

        /// <summary>
        /// Subscribe to outbound integration change notifications.
        /// Returns a subscription object whose ChangedEvent will be signalled
        /// whenever an outbound integration is created, edited or deleted.
        /// </summary>
        public static OutboundIntegrationChangeSubscription SubscribeToOutboundIntegrationChanges()
        {
            var sub = new OutboundIntegrationChangeSubscription();
            lock (outboundChangeSubscriptions)
            {
                outboundChangeSubscriptions.Add(sub);
            }
            return sub;
        }

        /// <summary>
        /// Unsubscribe from outbound integration change notifications.
        /// </summary>
        public static void UnsubscribeFromOutboundIntegrationChanges(OutboundIntegrationChangeSubscription subscription)
        {
            lock (outboundChangeSubscriptions)
            {
                outboundChangeSubscriptions.Remove(subscription);
            }
        }

        /// <summary>
        /// Notify all subscribers that the outbound integration configuration has changed.
        /// </summary>
        private static void NotifyOutboundIntegrationChange()
        {
            lock (outboundChangeSubscriptions)
            {
                foreach (var sub in outboundChangeSubscriptions)
                {
                    sub.ChangedEvent.Set();
                }
            }
        }

        #endregion

        #region Integration connection change subscriptions

        /// <summary>
        /// Represents a subscriber that wants to be notified when the integration
        /// connection configuration changes (create, edit or delete).
        /// </summary>
        public class IntegrationConnectionChangeSubscription
        {
            /// <summary>
            /// Signalled whenever an integration connection is created, edited or deleted.
            /// Subscribers can wait on this event to react promptly to changes.
            /// </summary>
            public AutoResetEvent ChangedEvent { get; } = new(false);
        }

        private static readonly List<IntegrationConnectionChangeSubscription> connectionChangeSubscriptions = new();

        /// <summary>
        /// Subscribe to integration connection change notifications.
        /// Returns a subscription object whose ChangedEvent will be signalled
        /// whenever an integration connection is created, edited or deleted.
        /// </summary>
        public static IntegrationConnectionChangeSubscription SubscribeToIntegrationConnectionChanges()
        {
            var sub = new IntegrationConnectionChangeSubscription();
            lock (connectionChangeSubscriptions)
            {
                connectionChangeSubscriptions.Add(sub);
            }
            return sub;
        }

        /// <summary>
        /// Unsubscribe from integration connection change notifications.
        /// </summary>
        public static void UnsubscribeFromIntegrationConnectionChanges(IntegrationConnectionChangeSubscription subscription)
        {
            lock (connectionChangeSubscriptions)
            {
                connectionChangeSubscriptions.Remove(subscription);
            }
        }

        /// <summary>
        /// Notify all subscribers that the integration connection configuration has changed.
        /// </summary>
        private static void NotifyIntegrationConnectionChange()
        {
            lock (connectionChangeSubscriptions)
            {
                foreach (var sub in connectionChangeSubscriptions)
                {
                    sub.ChangedEvent.Set();
                }
            }
        }

        #endregion

        #region Integration connections

        public List<IntegrationConnectionDTO> EnumerateIntegrationConnections()
        {
            return Context.IntegrationDao.EnumerateIntegrationConnections();
        }

        public List<IntegrationConnectionTypeDTO> EnumerateIntegrationConnectionTypes()
        {
            return Enum.GetValues<IntegrationConnectionType>()
                .Where(item => item != IntegrationConnectionType.Unknown)
                .Select(item => new IntegrationConnectionTypeDTO()
                {
                    integrationConnectionType = item,
                    description = GetIntegrationConnectionTypeDescription(item)
                })
                .ToList();
        }

        private string GetIntegrationConnectionTypeDescription(IntegrationConnectionType type)
        {
            return type switch
            {
                IntegrationConnectionType.MQTT => "MQTT",
                IntegrationConnectionType.HTTPPost => "HTTP Post",
                _ => type.ToString()
            };
        }

        public IntegrationConnectionDTO GetIntegrationConnection(int id)
        {
            var result = Context.IntegrationDao.GetIntegrationConnection(id);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Integrations.INVALID_INTEGRATION_CONNECTION_ID);
            return result;
        }

        private void ValidateIntegrationConnection(IntegrationConnectionDTO connection)
        {
            if (string.IsNullOrWhiteSpace(connection.description))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Common.Description));

            if (connection.connectionType == IntegrationConnectionType.HTTPPost)
            {
                if (string.IsNullOrWhiteSpace(connection.httpBaseUrl))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA("HTTP Base URL"));
                if (!Utils.ValidateHttpUrl(connection.httpBaseUrl))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_PARAMETER_VALUE("HTTP Base URL", connection.httpBaseUrl));
                
                if (connection.httpHeaders != null && connection.httpHeaders.Count > 0)
                {
                    foreach (var header in connection.httpHeaders)
                    {
                        if (string.IsNullOrWhiteSpace(header.key))
                            throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Integrations.HttpHeaderName));
                        if (string.IsNullOrWhiteSpace(header.value))
                            throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Integrations.HttpHeaderValue));
                    }
                }
            }

            if (connection.connectionType == IntegrationConnectionType.MQTT)
            {
                if (string.IsNullOrWhiteSpace(connection.mqttHost))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Integrations.MqttHost));
                if (!Utils.SplitMqttHost(connection.mqttHost, out var hostName, out var port))
                    throw new ControllerException(Context.ErrorCodes.General.INVALID_PARAMETER_VALUE(Context.Literals.Integrations.MqttHost, connection.mqttHost));
                connection.mqttHost = $"{hostName}:{port}";
            }
        }

        public IntegrationConnectionDTO CreateIntegrationConnection(IntegrationConnectionDTO dto)
        {
            ValidateIntegrationConnection(dto);
            Context.IntegrationDao.CreateIntegrationConnection(dto);
            NotifyIntegrationConnectionChange();
            return Context.IntegrationDao.GetIntegrationConnection(dto.integrationConnectionId);
        }

        public IntegrationConnectionDTO EditIntegrationConnection(IntegrationConnectionDTO dto)
        {
            GetIntegrationConnection(dto.integrationConnectionId);
            ValidateIntegrationConnection(dto);
            Context.IntegrationDao.EditIntegrationConnection(dto);
            NotifyIntegrationConnectionChange();
            return GetIntegrationConnection(dto.integrationConnectionId);
        }

        public void DeleteIntegrationConnection(int integrationConnectionId)
        {
            GetIntegrationConnection(integrationConnectionId);
            if (Context.IntegrationDao.HasOutboundIntegrations(integrationConnectionId))
                throw new ControllerException(Context.ErrorCodes.Integrations.INTEGRATION_CONNECTION_IN_USE);
            Context.IntegrationDao.DeleteIntegrationConnection(integrationConnectionId);
            NotifyIntegrationConnectionChange();
        }

        #endregion

        #region Outbound integrations

        public List<OutboundIntegrationDTO> EnumerateOutboundIntegrations()
        {
            return Context.IntegrationDao.EnumerateOutboundIntegrations();
        }

        public OutboundIntegrationDTO GetOutboundIntegration(int id)
        {
            var result = Context.IntegrationDao.GetOutboundIntegration(id);
            if (result == null)
                throw new ControllerException(Context.ErrorCodes.Integrations.INVALID_OUTBOUND_INTEGRATION_ID);
            return result;
        }

        private void ValidateOutboundIntegration(OutboundIntegrationDTO dto)
        {
            if (dto.sendIntervalSeconds < 1)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Integrations.SendIntervalSeconds));
            if (dto.reportBufferingMinutes < 0)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Integrations.ReportBufferingMinutes));
            if (string.IsNullOrWhiteSpace(dto.reportTemplate))
                throw new ControllerException(Context.ErrorCodes.General.INVALID_MANDATORY_DATA(Context.Literals.Integrations.Template));
        }

        public OutboundIntegrationDTO CreateOutboundIntegration(OutboundIntegrationDTO dto)
        {
            ValidateOutboundIntegration(dto);
            Context.IntegrationDao.CreateOutboundIntegration(dto);
            NotifyOutboundIntegrationChange();
            return Context.IntegrationDao.GetOutboundIntegration(dto.integrationId);
        }

        public OutboundIntegrationDTO EditOutboundIntegration(OutboundIntegrationDTO dto)
        {
            var existing = Context.IntegrationDao.GetOutboundIntegration(dto.integrationId);
            if (existing == null)
                throw new ControllerException(Context.ErrorCodes.General.INVALID_PARAMETER_VALUE("integrationId", dto.integrationId.ToString()));
            ValidateOutboundIntegration(dto);
            Context.IntegrationDao.EditOutboundIntegration(dto);
            NotifyOutboundIntegrationChange();
            return Context.IntegrationDao.GetOutboundIntegration(dto.integrationId);
        }

        public void DeleteOutboundIntegration(int integrationId)
        {
            Context.IntegrationDao.DeleteOutboundIntegration(integrationId);
            NotifyOutboundIntegrationChange();
        }

        public long AddOutboundIntegrationReport(int integrationId, DateTime createdUtc, string payload)
        {
            return Context.IntegrationDao.AddOutboundIntegrationReport(integrationId, createdUtc, payload);
        }

        public OutboundIntegrationReportDTO GetNextOutboundIntegrationReport(int integrationId)
        {
            return Context.IntegrationDao.GetNextOutboundIntegrationReport(integrationId);
        }

        public void DeleteOutboundIntegrationReport(long reportId)
        {
            Context.IntegrationDao.DeleteOutboundIntegrationReport(reportId);
        }

        #endregion
    }
}
