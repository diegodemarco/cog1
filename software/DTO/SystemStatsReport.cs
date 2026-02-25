namespace cog1
{
    public class SystemStatsReport
    {
        public DateTimeReportDTO dateTime { get; set; }
        public CPUReportDTO cpuReport { get; set; }
        public MemoryReportDTO memory { get; set; }
        public DiskReportDTO disk { get; set; }
        public TemperatureReport temperature { get; set; }
        public WiFiReport wiFi { get; set; }
        public EthernetReportDTO ethernet { get; set; }
    }
}
