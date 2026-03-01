namespace cog1
{
    public class SystemStatsReport
    {
        public DateTimeReportDTO dateTime { get; set; }
        public CPUReportDTO cpuReport { get; set; }
        public MemoryReportDTO memory { get; set; }
        public DiskReportDTO disk { get; set; }
        public TemperatureReportDTO temperature { get; set; }
        public WiFiReportDTO wiFi { get; set; }
        public EthernetReportDTO ethernet { get; set; }
    }
}
