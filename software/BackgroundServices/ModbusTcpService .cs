using cog1.DTO;
using cog1.Modbus;
using Microsoft.Extensions.Logging;

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
    public class ModbusTcpService(ILogger<ModbusTcpService> logger) : ModbusInterfaceBaseService(logger)
    {
        protected override string Description => "Modbus TCP";

        protected override ModbusServer CreateServer()
        {
            var result = new ModbusTcpServer();
            logger.LogInformation($"Successfully started Modbus TCP server");
            return result;
        }

        protected override long Dequeue(out ModbusRegisterDTO modbusRegister, out bool isRead, out double value)
        {
            return ModbusService.DequeueTcp(out modbusRegister, out isRead, out value);
        }

    }

}
