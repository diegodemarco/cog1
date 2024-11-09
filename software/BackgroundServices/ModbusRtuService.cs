using cog1.DTO;
using cog1.Modbus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace cog1.BackgroundServices
{

    /// <summary>
    /// The Modbus Rtu background service takes care of communicating with Modbus devices
    /// through RS-485. It checks the queue present in the Variable Polling Service and 
    /// performs the operations necessary to fulfill the requests in that queue.
    /// </summary>
    /// <param name="logger">
    /// Logger used by the background service.
    /// </param>
    /// <param name="scopeFactory">
    /// Scope factory used to create new scopes as needed, mostly to instantiate contexts 
    /// to access the database.
    /// </param>
    public class ModbusRtuService(ILogger<ModbusRtuService> logger) : BackgroundService
    {
        private ModbusRtuServer rtuServer = null;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Modbus RTU service started");

            // Signal that the background task has started, while postponing the first polling for 1 second
            await Utils.CancellableDelay(1000, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!CheckQueue())
                        await Utils.CancellableDelay(100, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error in Modbus RTU service: {ex}");
                    await Utils.CancellableDelay(5000, stoppingToken);
                }
            }

            logger.LogInformation("Modbus RTU service stopped");
        }

        private void EnsureServer()
        {
            if (rtuServer == null)
            {
                rtuServer = new ModbusRtuServer(Config.ModbusSerialPort, Config.ModbusBaudRate);
                logger.LogInformation($"Successfully started RTU server on {Config.ModbusSerialPort} at {Config.ModbusBaudRate} bauds");
            }
        }

        private void ProcessReadRequest(long operationId, ModbusRegisterDTO register)
        {
            try
            {
                EnsureServer();

                bool result;
                double value = 0;
                string errorMessage = "Unspecified error";
                switch (register.registerType)
                {
                    case ModbusRegisterType.Coil:
                        result = rtuServer.ReadCoil((byte)register.slaveId, (UInt16)register.registerAddress, out var coilValue);
                        if (result)
                            value = (coilValue) ? 1 : 0;
                        errorMessage = rtuServer.ErrorMessage;
                        break;
                    case ModbusRegisterType.DiscreteInput:
                        result = rtuServer.ReadDiscreteInput((byte)register.slaveId, (UInt16)register.registerAddress, out var discreteValue);
                        if (result)
                            value = (discreteValue) ? 1 : 0;
                        errorMessage = rtuServer.ErrorMessage;
                        break;
                    default:
                        result = false;
                        errorMessage = $"Unsupported register type {register.registerType}";
                        break;
                }
                if (result)
                {
                    ModbusService.CompleteOperation(operationId, value);
                }
                else
                {
                    ModbusService.CompleteOperation(operationId, errorMessage);
                }
            }
            catch (Exception ex)
            {
                ModbusService.CompleteOperation(operationId, ex.Message);
                throw;
            }
        }

        private void ProcessWriteRequest(long operationId, ModbusRegisterDTO register, double value)
        {
            try
            {
                EnsureServer();

                bool result;
                string errorMessage = "Unspecified error";
                switch (register.registerType)
                {
                    case ModbusRegisterType.Coil:
                        result = rtuServer.WriteCoil((byte)register.slaveId, (UInt16)register.registerAddress, value != 0);
                        errorMessage = rtuServer.ErrorMessage;
                        break;
                    //case ModbusRegisterType.DiscreteInput:
                    //    result = rtuServer.WriteDiscreteInput((byte)register.slaveId, (UInt16)register.registerAddress, out var discreteValue);
                    //    errorMessage = rtuServer.ErrorMessage;
                    //    break;
                    default:
                        result = false;
                        errorMessage = $"Unsupported register type {register.registerType}";
                        break;
                }
                if (result)
                {
                    ModbusService.CompleteOperation(operationId, value);
                }
                else
                {
                    ModbusService.CompleteOperation(operationId, errorMessage);
                }
            }
            catch (Exception ex)
            {
                ModbusService.CompleteOperation(operationId, ex.Message);
                throw;
            }
        }

        private bool CheckQueue()
        {
            var operationId = ModbusService.DequeueRtu(out var modbusRegister, out var isRead, out var value);
            if (operationId > 0)
            {
                if (isRead)
                {
                    ProcessReadRequest(operationId, modbusRegister);
                }
                else
                {
                    ProcessWriteRequest(operationId, modbusRegister, value);
                }
                return true;
            }

            return false;
        }

    }

}
