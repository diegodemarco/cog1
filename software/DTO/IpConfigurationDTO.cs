namespace cog1.DTO
{
    public class IpConfigurationDTO
    {
        public bool dhcp { get; set; }
        public string ipv4 { get; set; }
        public int netMask { get; set; }
        public string gateway { get; set; }
        public string dns { get; set; }
    }

}
