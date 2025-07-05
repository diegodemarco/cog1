using cog1.DTO;

namespace cog1
{
    public class EthernetReport
    {
        public string macAddress { get; set; }
        public int connectionState { get; set; }
        public bool isConnected { get; set; }
        public int speed { get; set; }
        public bool fullDuplex { get; set; }
        public bool autoNegotiate { get; set; }
        public IpConfigurationDTO ipConfiguration { get; set; }
    }
}
