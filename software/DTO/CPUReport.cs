namespace cog1app
{
    public class CPUUsageInterval
    {
        public double? idlePercentage { get; set; }
        public double? ioWaitPercentage { get; set; }
    }

    public class CPUUsage
    {
        public CPUUsageInterval lastSecond { get; set; }
        public CPUUsageInterval last5Seconds { get; set; }
        public CPUUsageInterval lastMinute { get; set; }
        public CPUUsageInterval last5Minutes { get; set; }
    }

    public class CPUReport
    {
        public CPUUsage usage {  get; set; }
        public string architecture { get; set; }
        public string vendor { get; set; }
        public string model { get; set; }
        public int cpuCount { get; set; }
    }
}
