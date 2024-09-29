using System.Collections.Generic;

namespace cog1
{
    public class WiFiReport
    {
        public string ssid { get; set; }
        public int connectionState { get; set; }
        public bool isConnected { get; set; }
        public string ipv4 { get; set; }
        public int maskBits { get; set; }
        public int rssi { get; set; }
        public int noise { get; set; }
        public int frequency { get; set; }
        public List<string> savedConnections { get; set; }
    }
}
