using cog1.Entities;
using Newtonsoft.Json;
using System.IO;

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
            public string serialPort = "/dev/ttyS5";
            public int baudRate = 9600;
            public bool rtuEnabled = true;
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
        public static string ModbusSerialPort { get => _config.modbus.serialPort; set { _config.modbus.serialPort = value; StoreConfig(); } }
        public static int ModbusBaudRate { get => _config.modbus.baudRate; set { _config.modbus.baudRate = value; StoreConfig(); } }
        public static bool ModbusRtuEnabled { get => _config.modbus.rtuEnabled; set { _config.modbus.rtuEnabled = value; StoreConfig(); } }

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