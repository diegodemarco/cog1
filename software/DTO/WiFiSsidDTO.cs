using System.Collections.Generic;

namespace cog1
{
    public class WiFiSsidDTO
    {
        public string ssid { get; set; }
        public bool isConnected { get; set; }
        public bool isSaved { get; set; }
        public int quality { get; set; }
        public string frequencies { get; set; }
        public bool isOpen { get; set; }
    }
}
