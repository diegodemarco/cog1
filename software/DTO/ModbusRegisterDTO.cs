namespace cog1.DTO
{
    public class ModbusRegisterDTO
    {
        public string tcpHost { get; set; }
        public int slaveId { get; set; }
        public int registerAddress { get; set; }
        public ModbusRegisterType registerType { get; set; }
        public ModbusDataType dataType { get; set; }
    }
}