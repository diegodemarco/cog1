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
                rtuServer = new ModbusRtuServer(
                    Config.ModbusRtuSerialPort, 
                    Config.ModbusRtuBaudRate,
                    Config.ModbusRtuDataBits,
                    Config.ModbusRtuStopBits,
                    Config.ModbusRtuParity);
                logger.LogInformation($"Successfully started RTU server on {Config.ModbusRtuSerialPort} at {Config.ModbusRtuBaudRate} bauds");
            }
        }

        private void ProcessReadRequest(long operationId, ModbusRegisterDTO register)
        {
            try
            {
                EnsureServer();

                bool result;
                bool boolValue;
                Int16 int16Value;
                UInt16 uint16Value;
                Int32 int32Value;
                UInt32 uint32Value;
                Single float32Value;
                double value = 0;
                string errorMessage = "Unspecified error";
                switch (register.registerType)
                {
                    case ModbusRegisterType.Coil:
                        result = rtuServer.ReadCoil((byte)register.slaveId, (UInt16)register.registerAddress, out boolValue);
                        if (result)
                            value = (boolValue) ? 1 : 0;
                        errorMessage = rtuServer.ErrorMessage;
                        break;
                    case ModbusRegisterType.DiscreteInput:
                        result = rtuServer.ReadDiscreteInput((byte)register.slaveId, (UInt16)register.registerAddress, out boolValue);
                        if (result)
                            value = (boolValue) ? 1 : 0;
                        errorMessage = rtuServer.ErrorMessage;
                        break;
                    case ModbusRegisterType.HoldingRegister:
                        switch (register.dataType)
                        {
                            case ModbusDataType.Boolean:
                                result = rtuServer.ReadHoldingRegisterUInt16((byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = (uint16Value == 0) ? 0 : 1;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.UInt16:
                                result = rtuServer.ReadHoldingRegisterUInt16((byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = uint16Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Int16:
                                result = rtuServer.ReadHoldingRegisterInt16((byte)register.slaveId, (UInt16)register.registerAddress, out int16Value);
                                if (result)
                                    value = int16Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.UInt32:
                                result = rtuServer.ReadHoldingRegisterUInt32((byte)register.slaveId, (UInt16)register.registerAddress, out uint32Value);
                                if (result)
                                    value = uint32Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Int32:
                                result = rtuServer.ReadHoldingRegisterInt32((byte)register.slaveId, (UInt16)register.registerAddress, out int32Value);
                                if (result)
                                    value = int32Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Float32:
                                result = rtuServer.ReadHoldingRegisterFloat32((byte)register.slaveId, (UInt16)register.registerAddress, out float32Value);
                                if (result)
                                    value = float32Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            default:
                                result = false;
                                errorMessage = $"Modbus read: unsupported holding register data type: {register.dataType}";
                                break;
                        }
                        break;
                    case ModbusRegisterType.InputRegister:
                        switch (register.dataType)
                        {
                            case ModbusDataType.Boolean:
                                result = rtuServer.ReadInputRegisterUInt16((byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = (uint16Value == 0) ? 0 : 1;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.UInt16:
                                result = rtuServer.ReadInputRegisterUInt16((byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = uint16Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Int16:
                                result = rtuServer.ReadInputRegisterInt16((byte)register.slaveId, (UInt16)register.registerAddress, out int16Value);
                                if (result)
                                    value = int16Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.UInt32:
                                result = rtuServer.ReadInputRegisterUInt32((byte)register.slaveId, (UInt16)register.registerAddress, out uint32Value);
                                if (result)
                                    value = uint32Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Int32:
                                result = rtuServer.ReadInputRegisterInt32((byte)register.slaveId, (UInt16)register.registerAddress, out int32Value);
                                if (result)
                                    value = int32Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Float32:
                                result = rtuServer.ReadInputRegisterFloat32((byte)register.slaveId, (UInt16)register.registerAddress, out float32Value);
                                if (result)
                                    value = float32Value;
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            default:
                                result = false;
                                errorMessage = $"Modbus read: unsupported input register data type: {register.dataType}";
                                break;
                        }
                        break;
                    default:
                        result = false;
                        errorMessage = $"Modbus read: unsupported register type {register.registerType}";
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
                    case ModbusRegisterType.HoldingRegister:
                        switch (register.dataType)
                        {
                            case ModbusDataType.Boolean:
                                result = rtuServer.WriteHoldingRegisterUInt16((byte)register.slaveId, (UInt16)register.registerAddress, (value == 0) ? (UInt16)0 : (UInt16)1);
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.UInt16:
                                result = rtuServer.WriteHoldingRegisterUInt16((byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToUInt16(value));
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Int16:
                                result = rtuServer.WriteHoldingRegisterInt16((byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToInt16(value));
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.UInt32:
                                result = rtuServer.WriteHoldingRegisterUInt32((byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToUInt32(value));
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Int32:
                                result = rtuServer.WriteHoldingRegisterInt32((byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToInt32(value));
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            case ModbusDataType.Float32:
                                result = rtuServer.WriteHoldingRegisterFloat32((byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToUInt32(value));
                                errorMessage = rtuServer.ErrorMessage;
                                break;
                            default:
                                result = false;
                                errorMessage = $"Modbus write: unsupported holding register data type: {register.dataType}";
                                break;
                        }
                        break;
                    default:
                        result = false;
                        errorMessage = $"Modbus write: unsupported register type {register.registerType}";
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
