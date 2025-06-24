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
        public bool dhcp { get; set; }
        public string ipv4 { get; set; }
        public int maskBits { get; set; }
        public string dns { get; set; }
        public string gateway { get; set; }
    }
}
