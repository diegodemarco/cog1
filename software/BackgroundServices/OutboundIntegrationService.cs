using cog1.Business;
using cog1.DTO;
using cog1.System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.BackgroundServices
{
    /// <summary>
    /// Background service that manages outbound integrations. Each outbound
    /// integration runs in its own dedicated thread, sending data to the cloud
    /// via HTTP POST or MQTT depending on the connection configuration.
    /// 
    /// Integrations send data:
    ///   - Periodically, based on <c>sendIntervalSeconds</c>.
    ///   - When one or more variables in <c>variableChangeList</c> change value.
    /// 
    /// The service reacts to CRUD operations on outbound integrations by
    /// subscribing to change notifications via
    /// <see cref="IntegrationBusiness.SubscribeToOutboundIntegrationChanges"/>.
    /// </summary>
    public class OutboundIntegrationService(ILogger<OutboundIntegrationService> logger, IServiceScopeFactory scopeFactory) : BaseBackgroundService(logger, scopeFactory, "Outbound integrations", LogCategory.Integrations)
    {
        #region Config change subscriptions

        private IntegrationBusiness.OutboundIntegrationChangeSubscription outboundChangeSubscription;
        private IntegrationBusiness.IntegrationConnectionChangeSubscription connectionChangeSubscription;

        #endregion

        #region Worker tracking

        /// <summary>
        /// Holds the state for a single outbound-integration worker thread.
        /// </summary>
        private class WorkerState
        {
            public OutboundIntegrationDTO Integration { get; set; }
            public IntegrationConnectionDTO Connection { get; set; }
            public CancellationTokenSource Cts { get; set; }
            public Task Task { get; set; }
            public IOManager.VariableChangeSubscription VariableSubscription { get; set; }
        }

        private readonly Dictionary<int, WorkerState> workers = new();

        #endregion

        protected override async Task Run(CancellationToken stoppingToken)
        {
            // Subscribe to outbound integration and connection configuration changes
            outboundChangeSubscription = IntegrationBusiness.SubscribeToOutboundIntegrationChanges();
            connectionChangeSubscription = IntegrationBusiness.SubscribeToIntegrationConnectionChanges();

            try
            {
                await Task.Yield();
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        ReconcileWorkers();

                        // Wait for a config change signal, service stop, or a 30s timeout.
                        // The timeout acts as a safety net to reconcile workers even if
                        // a change notification is somehow missed.
                        WaitHandle.WaitAny(
                            new[] { outboundChangeSubscription.ChangedEvent, connectionChangeSubscription.ChangedEvent, stoppingToken.WaitHandle },
                            TimeSpan.FromSeconds(30));
                    }
                    catch (Exception ex)
                    {
                        LogError($"Error in outbound integration service: {ex}");
                        Utils.CancellableDelay(5000, stoppingToken);
                    }
                }
            }
            finally
            {
                // Shut down all workers and unsubscribe
                StopAllWorkers();
                IntegrationBusiness.UnsubscribeFromOutboundIntegrationChanges(outboundChangeSubscription);
                IntegrationBusiness.UnsubscribeFromIntegrationConnectionChanges(connectionChangeSubscription);
            }
        }

        #region Worker reconciliation

        /// <summary>
        /// Loads the current outbound integrations from the database and
        /// starts / stops / updates worker threads to match.
        /// </summary>
        private void ReconcileWorkers()
        {
            // Load current state from the database
            List<OutboundIntegrationDTO> integrations;
            Dictionary<int, IntegrationConnectionDTO> connectionsMap;

            using (var scope = scopeFactory.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetService<Cog1Context>();
                integrations = ctx.IntegrationBusiness.EnumerateOutboundIntegrations();
                var connections = ctx.IntegrationBusiness.EnumerateIntegrationConnections();
                connectionsMap = connections.ToDictionary(c => c.integrationConnectionId);
            }

            var desiredIds = new HashSet<int>(integrations.Select(i => i.integrationId));

            // Stop workers for deleted integrations
            var toRemove = workers.Keys.Where(id => !desiredIds.Contains(id)).ToList();
            foreach (var id in toRemove)
            {
                LogInformation($"Stopping outbound integration worker for integration {id} (deleted)");
                StopWorker(id);
            }

            foreach (var integration in integrations)
            {
                if (!connectionsMap.TryGetValue(integration.integrationConnectionId, out var connection))
                {
                    // Connection not found; skip (and stop existing worker if any)
                    if (workers.ContainsKey(integration.integrationId))
                    {
                        LogWarning($"Stopping outbound integration worker for integration {integration.integrationId}: connection {integration.integrationConnectionId} not found");
                        StopWorker(integration.integrationId);
                    }
                    continue;
                }

                if (workers.TryGetValue(integration.integrationId, out var existing))
                {
                    // Check if the configuration changed
                    if (IntegrationConfigChanged(existing, integration, connection))
                    {
                        LogInformation($"Restarting outbound integration worker for integration {integration.integrationId} (configuration changed)");
                        StopWorker(integration.integrationId);
                        StartWorker(integration, connection);
                    }
                }
                else
                {
                    // New integration — start a worker
                    LogInformation($"Starting outbound integration worker for integration {integration.integrationId}");
                    StartWorker(integration, connection);
                }
            }
        }

        private static bool IntegrationConfigChanged(WorkerState state, OutboundIntegrationDTO integration, IntegrationConnectionDTO connection)
        {
            // Compare the serialized representations for a reliable deep comparison
            return JsonConvert.SerializeObject(state.Integration) != JsonConvert.SerializeObject(integration)
                || JsonConvert.SerializeObject(state.Connection) != JsonConvert.SerializeObject(connection);
        }

        private void StartWorker(OutboundIntegrationDTO integration, IntegrationConnectionDTO connection)
        {
            var cts = new CancellationTokenSource();
            var subscription = IOManager.SubscribeToVariableChanges();

            var state = new WorkerState
            {
                Integration = integration,
                Connection = connection,
                Cts = cts,
                VariableSubscription = subscription,
            };

            state.Task = Task.Run(() => WorkerLoop(state, cts.Token));
            workers[integration.integrationId] = state;
        }

        private void StopWorker(int integrationId)
        {
            if (workers.TryGetValue(integrationId, out var state))
            {
                state.Cts.Cancel();
                IOManager.UnsubscribeFromVariableChanges(state.VariableSubscription);
                try { state.Task.Wait(TimeSpan.FromSeconds(5)); } catch { /* expected */ }
                state.Cts.Dispose();
                workers.Remove(integrationId);
            }
        }

        private void StopAllWorkers()
        {
            foreach (var id in workers.Keys.ToList())
                StopWorker(id);
        }

        #endregion

        #region Worker loop

        /// <summary>
        /// Main loop for a single outbound integration worker.
        /// Sends data periodically and also when watched variables change.
        /// </summary>
        private async Task WorkerLoop(WorkerState state, CancellationToken ct)
        {
            var integration = state.Integration;
            var connection = state.Connection;
            var sw = Stopwatch.StartNew();
            var sendIntervalMs = (long)integration.sendIntervalSeconds * 1000;
            var watchesVariables = integration.variableChangeList != null && integration.variableChangeList.Count > 0;

            LogInformation($"Outbound integration worker {integration.integrationId} ({integration.description}) running");

            // Create a shared HttpClient for the lifetime of this worker (only for HTTP connections)
            using var httpClient = connection.connectionType == IntegrationConnectionType.HTTPPost
                ? CreateHttpClient(connection)
                : null;

            // Create a shared MQTT client for the lifetime of this worker (only for MQTT connections)
            IMqttClient mqttClient = null;
            MqttClientOptions mqttOptions = null;
            if (connection.connectionType == IntegrationConnectionType.MQTT)
                (mqttClient, mqttOptions) = CreateMqttClient(integration, connection);
            using var mqttClientDisposable = mqttClient;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    // Calculate how long until the next periodic send
                    var remainingMs = Math.Max(0, sendIntervalMs - sw.ElapsedMilliseconds);

                    // Wait for either: a variable change, the periodic timer, or cancellation
                    var waitHandles = new WaitHandle[] { state.VariableSubscription.ChangedEvent, ct.WaitHandle };
                    WaitHandle.WaitAny(waitHandles, (int)Math.Min(remainingMs, int.MaxValue));

                    if (ct.IsCancellationRequested)
                        break;

                    bool shouldSend = false;

                    // Check if a periodic send is due
                    if (sw.ElapsedMilliseconds >= sendIntervalMs)
                    {
                        sw.Restart();
                        shouldSend = true;
                    }

                    // Check for variable change triggers
                    var changedSet = DrainVariableChanges(state.VariableSubscription);
                    if (!shouldSend && watchesVariables)
                    {
                        if (changedSet.Any(id => integration.variableChangeList.Contains(id)))
                        {
                            sw.Restart();
                            LogInformation($"Outbound integration {integration.integrationId}: triggered by variable changes ({string.Join(", ", changedSet)})");
                            shouldSend = true;
                        }
                    }

                    if (shouldSend)
                    {
                        var payload = BuildPayload(integration);
                        await SendPayload(integration, connection, payload, httpClient, mqttClient, mqttOptions, ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    LogError($"Error in outbound integration worker {integration.integrationId}: {ex.Message}");
                    Utils.CancellableDelay(5000, ct);
                }
            }

            LogInformation($"Outbound integration worker {integration.integrationId} stopped");
        }

        /// <summary>
        /// Drains all pending variable change IDs from the subscription
        /// and returns the set of changed variable IDs.
        /// </summary>
        private static HashSet<int> DrainVariableChanges(IOManager.VariableChangeSubscription subscription)
        {
            var result = new HashSet<int>(subscription.Changes.Keys);
            // Do not just "clear" the dictionary, as new changes might come in concurrently; 
            // instead, remove drained IDs one by one
            foreach (var id in result)
                subscription.Changes.TryRemove(id, out _);
            return result;
        }

        #endregion

        #region Payload building

        /// <summary>
        /// Builds the payload string for the given outbound integration
        /// by rendering the report template with the current variable values.
        /// 
        /// The template can contain placeholders in the form <c>{variableId}</c>
        /// that are replaced with the current value of the corresponding variable.
        /// Additionally, <c>{timestamp}</c> is replaced with the current UTC time
        /// in ISO-8601 format.
        /// 
        /// If the template is empty, a default JSON payload is generated containing
        /// all variable values.
        /// </summary>
        private static string BuildPayload(OutboundIntegrationDTO integration)
        {
            var variableValues = IOManager.GetVariableValues();
            var template = integration.reportTemplate;

            if (string.IsNullOrWhiteSpace(template))
            {
                // Default: send all current variable values as JSON
                var data = variableValues
                    .OrderBy(kv => kv.Key)
                    .Select(kv => new
                    {
                        variableId = kv.Key,
                        value = kv.Value.value,
                        lastUpdateUtc = kv.Value.lastUpdateUtc?.ToString("o")
                    });
                return JsonConvert.SerializeObject(new
                {
                    timestamp = DateTime.UtcNow.ToString("o"),
                    variables = data
                }, Formatting.None);
            }

            // Render the template by replacing {variableId} placeholders
            var result = template;
            result = result.Replace("{timestamp}", DateTime.UtcNow.ToString("o"));

            foreach (var kv in variableValues)
            {
                result = result.Replace($"{{{kv.Key}}}", kv.Value.value?.ToString() ?? "null");
            }

            return result;
        }

        #endregion

        #region Sending

        /// <summary>
        /// Sends the payload using the appropriate transport (HTTP POST or MQTT)
        /// based on the connection type.
        /// </summary>
        private async Task SendPayload(OutboundIntegrationDTO integration, IntegrationConnectionDTO connection, string payload, HttpClient httpClient, IMqttClient mqttClient, MqttClientOptions mqttOptions, CancellationToken ct)
        {
            LogInformation($"Outbound integration {integration.integrationId}: sending payload via {connection.connectionType}");
            switch (connection.connectionType)
            {
                case IntegrationConnectionType.HTTPPost:
                    await SendHttp(integration, connection, payload, httpClient, ct);
                    break;
                case IntegrationConnectionType.MQTT:
                    await SendMqtt(integration, connection, payload, mqttClient, mqttOptions, ct);
                    break;
                default:
                    LogWarning($"Outbound integration {integration.integrationId}: unknown connection type {connection.connectionType}");
                    break;
            }
        }

        /// <summary>
        /// Sends the payload via HTTP POST, combining the connection's base URL
        /// with the integration's httpUrl (removing double slashes).
        /// </summary>
        private async Task SendHttp(OutboundIntegrationDTO integration, IntegrationConnectionDTO connection, string payload, HttpClient client, CancellationToken ct)
        {
            var url = CombinePaths(connection.httpBaseUrl, integration.httpUrl, '/');

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content, ct);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                LogWarning($"Outbound integration {integration.integrationId} HTTP POST to {url} returned {(int)response.StatusCode}: {body}");
            }
        }

        /// <summary>
        /// Sends the payload via MQTT, combining the connection's base topic
        /// with the integration's mqttTopic (removing double slashes).
        /// </summary>
        private async Task SendMqtt(OutboundIntegrationDTO integration, IntegrationConnectionDTO connection, string payload, IMqttClient client, MqttClientOptions mqttOptions, CancellationToken ct)
        {
            if (client == null) return;

            var topic = CombinePaths(connection.mqttBaseTopic, integration.mqttTopic, '/');

            try
            {
                if (!client.IsConnected)
                {
                    var result = await client.ConnectAsync(mqttOptions, ct);
                    if (result.ResultCode != MqttClientConnectResultCode.Success)
                    {
                        LogWarning($"Outbound integration {integration.integrationId} MQTT client failed to connect: {result.ResultCode}");
                        return;
                    }
                }

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();
                await client.PublishAsync(message, ct);
            }
            catch (Exception ex)
            {
                LogWarning($"Outbound integration {integration.integrationId} MQTT publish to {topic} failed: {ex.Message}");
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates an MQTT client and its connection options for the broker
        /// defined in the integration connection. Does not connect; the
        /// connection is established lazily in <see cref="SendMqtt"/>.
        /// </summary>
        private (IMqttClient client, MqttClientOptions options) CreateMqttClient(OutboundIntegrationDTO integration, IntegrationConnectionDTO connection)
        {
            if (!Utils.SplitMqttHost(connection.mqttHost, out var host, out var port))
            {
                LogWarning($"Outbound integration {integration.integrationId}: invalid MQTT host '{connection.mqttHost}'");
                return (null, null);
            }

            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(host, port)
                .WithClientId(Guid.NewGuid().ToString())
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(15));

            // Add credentials if provided
            if (!string.IsNullOrWhiteSpace(connection.userName))
                optionsBuilder.WithCredentials(connection.userName, connection.password ?? "");

            // Configure TLS if enabled
            if (connection.mqttUseTls)
            {
                bool hasServerCert = !string.IsNullOrWhiteSpace(connection.mqttServerCertificate);
                bool hasClientCert = !string.IsNullOrWhiteSpace(connection.mqttClientCertificate);
                optionsBuilder.WithTlsOptions(tls =>
                {
                    if (hasServerCert)
                    {
                        // Trust the server CA certificate provided in PEM format
                        var caCert = X509Certificate2.CreateFromPem(connection.mqttServerCertificate);
                        tls.WithCertificateValidationHandler(args =>
                        {
                            // Accept the server certificate if it matches the provided CA
                            if (args.Certificate != null)
                            {
                                using var chain = new X509Chain();
                                chain.ChainPolicy.ExtraStore.Add(caCert);
                                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                                return chain.Build(new X509Certificate2(args.Certificate));
                            }
                            return false;
                        });
                    }

                    if (hasClientCert)
                    {
                        // Provide the client certificate for mutual TLS authentication
                        var clientCert = X509Certificate2.CreateFromPem(connection.mqttClientCertificate);
                        tls.WithClientCertificates(new[] { clientCert });
                    }
                });
            }

            var factory = new MqttClientFactory();
            var client = factory.CreateMqttClient();

            return (client, optionsBuilder.Build());
        }

        /// <summary>
        /// Creates an HttpClient pre-configured with the connection's timeout,
        /// custom headers and basic auth credentials.
        /// </summary>
        private static HttpClient CreateHttpClient(IntegrationConnectionDTO connection)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            // Add custom headers from the connection
            if (connection.httpHeaders != null)
            {
                foreach (var header in connection.httpHeaders)
                {
                    if (!string.IsNullOrWhiteSpace(header.key))
                        client.DefaultRequestHeaders.TryAddWithoutValidation(header.key, header.value);
                }
            }

            // Add basic auth if configured
            if (!string.IsNullOrWhiteSpace(connection.userName))
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{connection.userName}:{connection.password ?? ""}"));
                client.DefaultRequestHeaders.Authorization = new global::System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
            }

            return client;
        }

        /// <summary>
        /// Combines a base path with a sub-path, ensuring exactly one separator between them.
        /// </summary>
        private static string CombinePaths(string basePath, string subPath, char separator)
        {
            basePath = (basePath ?? "").TrimEnd(separator);
            subPath = (subPath ?? "").TrimStart(separator);
            if (string.IsNullOrEmpty(basePath)) return subPath;
            if (string.IsNullOrEmpty(subPath)) return basePath;
            return basePath + separator + subPath;
        }

        #endregion
    }
}
