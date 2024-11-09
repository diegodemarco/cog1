namespace cog1.Modbus
{
    public class ModbusErrorInfo
    {
        public string ErrorMessage { get; set; }
        public byte ModbusErrorCode { get; set; }
        public bool IsModbusError => ModbusErrorCode != 0;
    }
}
