using System.Collections.Generic;

namespace cog1
{
    public class TemperatureReport
    {
        public double? maxTemperatureC { get; set; }
        public bool? isCritical { get; set; }
        public List<TemperatureReportEntry> details { get; set; }
}

    public class TemperatureReportEntry
    {
        public string source { get; set; }

        public double temperatureC { get; set; }

        public double? criticalTemperatureC { get; set; }

        public bool isCritical { get; set; }
    }
}
