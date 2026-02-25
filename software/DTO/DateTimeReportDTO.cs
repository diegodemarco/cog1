using System;

namespace cog1
{
    public class DateTimeReportDTO
    {
        public DateTime utc { get; set; }
        public DateTime local { get; set; }
        public string timeZone { get; set; }
        public long uptime { get; set; }
    }
}
