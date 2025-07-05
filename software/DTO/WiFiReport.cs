using cog1.DTO;
using System.Collections.Generic;

namespace cog1
{
    public class WiFiReport
    {
        public string macAddress { get; set; }
        public string ssid { get; set; }
        public int connectionState { get; set; }
        public bool isConnected { get; set; }
        public IpConfigurationDTO ipConfiguration { get; set; }
        public int rssi { get; set; }
        public int noise { get; set; }
        public int frequency { get; set; }
        public List<string> savedConnections { get; set; }
    }
}
