namespace cog1
{
    public class MemoryReportDTO
    {
        public long totalBytes { get; set; }
        public long usedBytes { get; set; }
        public long freeBytes { get; set; }
        public long availableBytes { get; set; }
        public double freePercentage { get; set; }
        public double availablePercentage { get; set; }
    }
}
