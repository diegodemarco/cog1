using cog1.DTO;
using cog1.Modbus;
using Microsoft.Extensions.Logging;

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
    public class ModbusRtuService(ILogger<ModbusRtuService> logger) : ModbusInterfaceBaseService(logger)
    {
        protected override string Description => "Modbus RTU";

        protected override ModbusServer CreateServer()
        {
            var result = new ModbusRtuServer(
                Config.ModbusRtuSerialPort,
                Config.ModbusRtuBaudRate,
                Config.ModbusRtuDataBits,
                Config.ModbusRtuStopBits,
                Config.ModbusRtuParity);
            logger.LogInformation($"Successfully started Modbus RTU server on {Config.ModbusRtuSerialPort}. Port setup: {Config.ModbusRtuBaudRate},{Config.ModbusRtuDataBits},{Config.ModbusRtuParity},{Config.ModbusRtuStopBits}");
            return result;
        }

        protected override long Dequeue(out ModbusRegisterDTO modbusRegister, out bool isRead, out double value)
        {
            return ModbusService.DequeueRtu(out modbusRegister, out isRead, out value);
        }

    }

}
