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
    /// The Modbus Tcp background service takes care of communicating with Modbus devices
    /// through TCP/IP. It checks the queue present in the Variable Polling Service and 
    /// performs the operations necessary to fulfill the requests in that queue.
    /// </summary>
    /// <param name="logger">
    /// Logger used by the background service.
    /// </param>
    public abstract class ModbusInterfaceBaseService(ILogger logger) : BackgroundService
    {
        private ModbusServer server;

        #region Abstract
        
        protected abstract string Description { get; }
        protected abstract long Dequeue(out ModbusRegisterDTO modbusRegister, out bool isRead, out double value);
        protected abstract ModbusServer CreateServer();

        #endregion

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{Description} service started");

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
                    logger.LogError($"Error in {Description} service: {ex}");
                    await Utils.CancellableDelay(5000, stoppingToken);
                }
            }

            logger.LogInformation($"{Description} service stopped");
        }

        private void EnsureServer()
        {
            if (server == null)
                server = CreateServer();
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
                        result = server.ReadCoil(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out boolValue);
                        if (result)
                            value = (boolValue) ? 1 : 0;
                        errorMessage = server.ErrorMessage;
                        break;
                    case ModbusRegisterType.DiscreteInput:
                        result = server.ReadDiscreteInput(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out boolValue);
                        if (result)
                            value = (boolValue) ? 1 : 0;
                        errorMessage = server.ErrorMessage;
                        break;
                    case ModbusRegisterType.HoldingRegister:
                        switch (register.dataType)
                        {
                            case ModbusDataType.Boolean:
                                result = server.ReadHoldingRegisterUInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = (uint16Value == 0) ? 0 : 1;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.UInt16:
                                result = server.ReadHoldingRegisterUInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = uint16Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Int16:
                                result = server.ReadHoldingRegisterInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out int16Value);
                                if (result)
                                    value = int16Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.UInt32:
                                result = server.ReadHoldingRegisterUInt32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out uint32Value);
                                if (result)
                                    value = uint32Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Int32:
                                result = server.ReadHoldingRegisterInt32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out int32Value);
                                if (result)
                                    value = int32Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Float32:
                                result = server.ReadHoldingRegisterFloat32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out float32Value);
                                if (result)
                                    value = float32Value;
                                errorMessage = server.ErrorMessage;
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
                                result = server.ReadInputRegisterUInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = (uint16Value == 0) ? 0 : 1;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.UInt16:
                                result = server.ReadInputRegisterUInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out uint16Value);
                                if (result)
                                    value = uint16Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Int16:
                                result = server.ReadInputRegisterInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out int16Value);
                                if (result)
                                    value = int16Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.UInt32:
                                result = server.ReadInputRegisterUInt32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out uint32Value);
                                if (result)
                                    value = uint32Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Int32:
                                result = server.ReadInputRegisterInt32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out int32Value);
                                if (result)
                                    value = int32Value;
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Float32:
                                result = server.ReadInputRegisterFloat32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, out float32Value);
                                if (result)
                                    value = float32Value;
                                errorMessage = server.ErrorMessage;
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
                        result = server.WriteCoil(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, value != 0);
                        errorMessage = server.ErrorMessage;
                        break;
                    case ModbusRegisterType.HoldingRegister:
                        switch (register.dataType)
                        {
                            case ModbusDataType.Boolean:
                                result = server.WriteHoldingRegisterUInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, (value == 0) ? (UInt16)0 : (UInt16)1);
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.UInt16:
                                result = server.WriteHoldingRegisterUInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToUInt16(value));
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Int16:
                                result = server.WriteHoldingRegisterInt16(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToInt16(value));
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.UInt32:
                                result = server.WriteHoldingRegisterUInt32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToUInt32(value));
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Int32:
                                result = server.WriteHoldingRegisterInt32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToInt32(value));
                                errorMessage = server.ErrorMessage;
                                break;
                            case ModbusDataType.Float32:
                                result = server.WriteHoldingRegisterFloat32(register.tcpHost, (byte)register.slaveId, (UInt16)register.registerAddress, Convert.ToUInt32(value));
                                errorMessage = server.ErrorMessage;
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
            var operationId = Dequeue(out var modbusRegister, out var isRead, out var value);
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
