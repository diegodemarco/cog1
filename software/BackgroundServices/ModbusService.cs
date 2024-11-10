using cog1.Dao;
using cog1.DTO;
using cog1.Hardware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.BackgroundServices
{

    /// <summary>
    /// The Modbus background service maintains the modbus operation queue
    /// and dispatches operations to/from the modbus RTU and modbus TCP
    /// background services.
    /// </summary>
    /// <param name="logger">
    /// Logger used by the background service.
    /// </param>
    /// <param name="scopeFactory">
    /// Scope factory used to create new scopes as needed, mostly to instantiate contexts 
    /// to access the database.
    /// </param>
    public class ModbusService(ILogger<ModbusService> logger) : BackgroundService
    {
        private static Random random = new Random();
        private static object _lock = new();
        private static List<ModbusQueueEntry> modbusQueue = new();

        private enum ModbusQueueItemState
        {
            Pending = 0,
            Processing = 1,
            Success = 2,
            Error = 3,
            Purge = 4,
        }

        private enum ModbusOperationType
        {
            Read = 0,
            Write = 1,
        }

        private class ModbusQueueEntry
        {
            public readonly long operationId = random.NextInt64();
            public ModbusOperationType operationType = ModbusOperationType.Read;
            public ModbusQueueItemState state = ModbusQueueItemState.Pending;
            public int variableId;
            public ModbusRegisterDTO modbusRegister;
            public double value;
            public string errorMessage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Modbus manager service started");

            // Signal that the background task has started, while postponing the first polling for 1 second
            await Utils.CancellableDelay(1000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CheckCompletedItems();
                    //lock (_lock)
                    //{
                    //    Console.WriteLine($"Modbus queue size: {modbusQueue.Count}");
                    //}
                    await Utils.CancellableDelay(1000, stoppingToken);

                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in Modbus manager service: {ex}");
                    await Utils.CancellableDelay(5000, stoppingToken);
                }
            }

            logger.LogInformation("Modbus manager service stopped");
        }

        private static void CheckCompletedItems()
        {
            var varsToUpdate = new Dictionary<int, double>();

            // Process items in the queue that are completed or ready to be purged
            lock (_lock)
            {
                // Remove purged items
                modbusQueue.RemoveAll(item => item.state == ModbusQueueItemState.Purge);

                // Process remaining items
                foreach (var item in modbusQueue)
                {
                    if (item.operationType == ModbusOperationType.Read)
                    {
                        switch (item.state)
                        {
                            case ModbusQueueItemState.Success:
                                varsToUpdate[item.variableId] = item.value;
                                item.state = ModbusQueueItemState.Purge;
                                break;
                            case ModbusQueueItemState.Error:
                                // Do something with the error
                                item.state = ModbusQueueItemState.Purge;
                                break;
                        }
                    }
                }
            }

            // Update variable value in the IO Manager
            foreach (var item in varsToUpdate)
            {
                IOManager.SetVariableValue(item.Key, item.Value);
            }
        }

        private static long DequeueOperation(Func<ModbusQueueEntry, bool> func, out ModbusRegisterDTO modbusRegister, out bool isRead, out double value)
        {
            lock (_lock)
            {
                if (modbusQueue.Count > 0)
                {
                    // Write operations take precedence
                    var item = modbusQueue.Find(item => item.state == ModbusQueueItemState.Pending && item.operationType == ModbusOperationType.Write && func(item));

                    // If no operation was found, search again without restrictions
                    if (item == null)
                        item = modbusQueue.Find(item => item.state == ModbusQueueItemState.Pending && func(item));

                    if (item != null)
                    {
                        item.state = ModbusQueueItemState.Processing;
                        modbusRegister = item.modbusRegister;
                        isRead = (item.operationType == ModbusOperationType.Read);
                        value = item.value;
                        return item.operationId;
                    }
                }

                // No louck
                modbusRegister = null;
                isRead = true;
                value = 0;
                return 0;
            }

        }

        public static long DequeueRtu(out ModbusRegisterDTO modbusRegister, out bool isRead, out double value)
        {
            return DequeueOperation(item => string.IsNullOrWhiteSpace(item.modbusRegister.tcpHost), out modbusRegister, out isRead, out value);
        }

        public static long DequeueTcp(out ModbusRegisterDTO modbusRegister, out bool isRead, out double value)
        {
            return DequeueOperation(item => !string.IsNullOrWhiteSpace(item.modbusRegister.tcpHost), out modbusRegister, out isRead, out value);
        }

        private static bool CancelOperation(long op)
        {
            lock (_lock)
            {
                var item = modbusQueue.Find(x => x.operationId == op);
                if (item != null)
                {
                    item.state = ModbusQueueItemState.Purge;
                    item.errorMessage = "Cancelled";
                    return true;
                }
            }
            return false;
        }

        private static bool WaitForOperation(long op, out bool success, out double value, out string errorMessage)
        {
            var sw = Stopwatch.StartNew();
            var timeout = sw.ElapsedMilliseconds + 10000;   // 10 second timeout
            while (sw.ElapsedMilliseconds < timeout)
            {
                lock (_lock)
                {
                    var item = modbusQueue.Find(x => (x.operationId == op) && (x.state == ModbusQueueItemState.Success || x.state == ModbusQueueItemState.Error));
                    if (item != null)
                    {
                        success = (item.state == ModbusQueueItemState.Success);
                        value = item.value;
                        errorMessage = item.errorMessage;
                        item.state = ModbusQueueItemState.Purge;
                        return true;
                    }
                }
                Thread.Sleep(10);
            }
            success = false;
            value = 0;
            errorMessage = "Internal timeout";
            CancelOperation(op);
            return false;
        }

        private static void _CompleteOperation(long operationId, bool success, double value, string error)
        {
            lock (_lock)
            {
                var item = modbusQueue.Find(item => (item.operationId == operationId) && (item.state == ModbusQueueItemState.Processing));
                if (item == null)
                    return;

                if (success)
                {
                    item.state = ModbusQueueItemState.Success;
                    item.value = value;
                }
                else
                {
                    item.state = ModbusQueueItemState.Error;
                    item.errorMessage = error;
                }
            }
            CheckCompletedItems();
        }

        public static void CompleteOperation(long operationId, double value)
        {
            _CompleteOperation(operationId, true, value, null);
        }

        public static void CompleteOperation(long operationId, string errorMessage)
        {
            _CompleteOperation(operationId, false, 0, errorMessage);
        }

        #region Read operations

        public static long QueueRead(VariableDao.BasicVariableDefinition v)
        {
            lock (_lock)
            {
                // It's not necessary to queue another read operation if there is already one in the queue
                if (modbusQueue.Any(item => item.operationType == ModbusOperationType.Read && item.variableId == v.variableId))
                    return 0;

                var item = new ModbusQueueEntry()
                {
                    operationType = ModbusOperationType.Read,
                    variableId = v.variableId,
                    modbusRegister = v.modbusRegister
                };
                modbusQueue.Add(item);
                return item.operationId;
            }
        }

        #endregion

        #region Write operations

        private static long QueueWrite(VariableDTO v, double value)
        {
            lock (_lock)
            {
                var item = new ModbusQueueEntry()
                {
                    operationType = ModbusOperationType.Write,
                    variableId = v.variableId,
                    modbusRegister = v.modbusRegister,
                    value = value
                };
                modbusQueue.Add(item);
                return item.operationId;
            }
        }

        public static bool WriteRegister(VariableDTO v, double value, out string errorMessage)
        {
            // Queue the write operation
            var op = QueueWrite(v, value);

            Console.WriteLine($"Queued Modbus write operation");

            // Wait for completion
            if (!WaitForOperation(op, out var success, out _, out errorMessage))
                return false;

            Console.WriteLine(success? "Modbus write operation successful" : "Modbus write operation failed");

            // Update the variable value
            IOManager.SetVariableValue(v.variableId, value);
            return success;
        }

        #endregion

    }

}
