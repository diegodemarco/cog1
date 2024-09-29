namespace cog1
{
    public class SystemStatsReport
    {
        public DateTimeReport dateTime { get; set; }
        public CPUReport cpuReport { get; set; }
        public MemoryReport memory { get; set; }
        public DiskReport disk { get; set; }
        public TemperatureReport temperature { get; set; }
        public WiFiReport wiFi { get; set; }
    }
}
