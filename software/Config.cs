using cog1.Entities;
using Newtonsoft.Json;
using System.IO;
using System.IO.Ports;

namespace cog1
{
    public static class Config
    {
        private static readonly string CONFIG_FILE_NAME = Path.Combine(Global.DataDirectory, "config.json");

        private class JsonConfig_Outputs_Startup
        {
            public OutputStartupType do1 = OutputStartupType.Restore;
            public OutputStartupType do2 = OutputStartupType.Restore;
            public OutputStartupType do3 = OutputStartupType.Restore;
            public OutputStartupType do4 = OutputStartupType.Restore;
        }

        private class JsonConfig_Outputs
        {
            public JsonConfig_Outputs_Startup startup = new();
        }

        private class JsonConfig_Modbus
        {
            public string rtuSerialPort = "/dev/ttyS5";
            public int rtuBaudRate = 9600;
            public bool rtuEnabled = true;
            public int rtuDataBits = 8;
            public StopBits rtuStopBits = StopBits.One;
            public Parity rtuParity = Parity.None;
        }

        private class JsonConfig
        {
            public JsonConfig_Outputs outputs = new();
            public JsonConfig_Modbus modbus = new();
        }

        private static JsonConfig _config = new();

        static Config()
        {
            LoadConfig();
        }

        // Output startup
        public static OutputStartupType DO1StartupType { get => _config.outputs.startup.do1; set { _config.outputs.startup.do1 = value; StoreConfig(); } }
        public static OutputStartupType DO2StartupType { get => _config.outputs.startup.do2; set { _config.outputs.startup.do2 = value; StoreConfig(); } }
        public static OutputStartupType DO3StartupType { get => _config.outputs.startup.do3; set { _config.outputs.startup.do3 = value; StoreConfig(); } }
        public static OutputStartupType DO4StartupType { get => _config.outputs.startup.do4; set { _config.outputs.startup.do4 = value; StoreConfig(); } }

        // Modbus RTU
        public static bool ModbusRtuEnabled { get => _config.modbus.rtuEnabled; set { _config.modbus.rtuEnabled = value; StoreConfig(); } }
        public static string ModbusRtuSerialPort { get => _config.modbus.rtuSerialPort; set { _config.modbus.rtuSerialPort = value; StoreConfig(); } }
        public static int ModbusRtuBaudRate { get => _config.modbus.rtuBaudRate; set { _config.modbus.rtuBaudRate = value; StoreConfig(); } }
        public static int ModbusRtuDataBits { get => _config.modbus.rtuDataBits; set { _config.modbus.rtuDataBits = value; StoreConfig(); } }
        public static StopBits ModbusRtuStopBits { get => _config.modbus.rtuStopBits; set { _config.modbus.rtuStopBits = value; StoreConfig(); } }
        public static Parity ModbusRtuParity { get => _config.modbus.rtuParity; set { _config.modbus.rtuParity = value; StoreConfig(); } }

        private static void LoadConfig()
        {
            if (File.Exists(CONFIG_FILE_NAME))
            {
                _config = JsonConvert.DeserializeObject<JsonConfig>(File.ReadAllText(CONFIG_FILE_NAME));
            }
            else
            {
                StoreConfig();
            }
        }
        
        private static void StoreConfig()
        {
            File.WriteAllText(CONFIG_FILE_NAME, JsonConvert.SerializeObject(_config));
        }
    }
}