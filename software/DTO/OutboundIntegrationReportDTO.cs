using System;

namespace cog1.DTO
{
    public class OutboundIntegrationReportDTO
    {
        public long reportId { get; set; }
        public int integrationId { get; set; }
        public DateTime createdUtc { get; set; }
        public string payload { get; set; }
    }
}
