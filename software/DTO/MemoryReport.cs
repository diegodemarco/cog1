namespace cog1
{
    public class MemoryReport
    {
        public long totalBytes { get; set; }
        public long usedBytes { get; set; }
        public long freeBytes { get; set; }

        public double freePercentage { get; set; }
    }
}
