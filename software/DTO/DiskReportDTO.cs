namespace cog1
{
    public class DiskReportDTO
    {
        public long bytesTotal { get; set; }
        public long bytesUsed { get; set; }
        public long bytesAvailable { get; set; }
        public double freePercentage { get; set; }
    }
}
