using cog1.Hardware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cog1
{
    public static class SystemStats
    {
        private class SensorsDataEntry
        {
            public string Adapter = null;
            public SensorsTemp1 temp1 = null;
        }

        private class SensorsTemp1
        {
            public double temp1_input = 0;
            public double? temp1_crit = null;
        }

        private class LsCPUData
        {
            public List<LsCPUEntry> lscpu = null;
        }

        private class LsCPUEntry
        {
            public string field = null;
            public string data = null;
        }

        private class CpuMeasurement
        {
            public long totalTime;
            public long idleTime;
            public long ioWaitTime;
        }

        private static object _lock = new();
        private static List<CpuMeasurement> cpu1SecMeasurements = new();

        public static TemperatureReport GetTemps()
        {
            var output = OSUtils.RunWithOutput("sensors", "-j");

            // Parse the output
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, SensorsDataEntry>>(output);
            var result = new TemperatureReport()
            {
                details = new List<TemperatureReportEntry>()
            };
            foreach (var item in jsonData.Where(x => x.Value.temp1 != null))
            {
                result.details.Add(new TemperatureReportEntry()
                {
                    source = item.Key,
                    temperatureC = item.Value.temp1.temp1_input,
                    criticalTemperatureC = item.Value.temp1.temp1_crit,
                    isCritical = (item.Value.temp1.temp1_crit != null && item.Value.temp1.temp1_input >= item.Value.temp1.temp1_crit.Value)
                });
            };
            if (result.details.Count > 0)
            {
                result.maxTemperatureC = result.details.Select(item => item.temperatureC).Max();
                result.isCritical = result.details.Any(item => item.isCritical);
            }

            // Done
            return result;
        }

        public static MemoryReport GetMem()
        {
            var lines = OSUtils.GetLines(OSUtils.RunWithOutput("free", "--bytes"));

            // Parse the output
            var line = lines.FirstOrDefault(l => l.ToLower().StartsWith("mem:"));
            if (line == null)
                return new MemoryReport();

            var lineParts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if ((lineParts.Length != 7) || !long.TryParse(lineParts[1], out var totalMem) 
                || !long.TryParse(lineParts[2], out var usedMem) || !long.TryParse(lineParts[3], out var freeMem))
                return new MemoryReport();

            // Done
            return new MemoryReport()
            {
                totalBytes = totalMem,
                usedBytes = usedMem,
                freeBytes = freeMem,
                freePercentage = Math.Round(100 * (double)freeMem / (double)totalMem, 2)
            };
        }

        public static long GetUptime()
        {
            var content = File.ReadAllText("/proc/uptime");
            var parts = content.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length < 2)
                return 0;
            return Convert.ToInt64(double.Parse(parts[0]));
        }

        public static CPUUsageInterval GetCpuUsage(int pastSeconds)
        {
            // Calculate CPU usage based on the last two 1-second reports
            lock (_lock)
            {
                if (cpu1SecMeasurements.Count >= 2)
                {
                    var m1Index = cpu1SecMeasurements.Count - pastSeconds - 1;
                    if (m1Index < 0)
                        m1Index = 0;
                    var m1 = cpu1SecMeasurements[m1Index];
                    var m2 = cpu1SecMeasurements[cpu1SecMeasurements.Count - 1];
                    var deltaTotal = m2.totalTime - m1.totalTime;
                    var deltaIdle = m2.idleTime - m1.idleTime;
                    var deltaioWait = m2.ioWaitTime - m1.ioWaitTime;
                    if (deltaTotal > 0)
                    {
                        return new CPUUsageInterval()
                        {
                            idlePercentage = Math.Round((deltaIdle * 100) / (double)deltaTotal, 2),
                            ioWaitPercentage = Math.Round((deltaioWait * 100) / (double)deltaTotal, 2)
                        };
                    }
                }
                return null;
            }
        }

        public static CPUReport GetCpuReport()
        {
            var result = new CPUReport();

            // Get CPU information
            var cpuData = JsonConvert.DeserializeObject<LsCPUData>(OSUtils.RunWithOutput("lscpu", "--json")).lscpu;
            foreach (var entry in cpuData)
            {
                if (string.Equals(entry.field, "Architecture:", StringComparison.OrdinalIgnoreCase))
                {
                    result.architecture = entry.data;
                }
                else if (string.Equals(entry.field, "Vendor ID:", StringComparison.OrdinalIgnoreCase))
                {
                    result.vendor = entry.data;
                }
                else if (string.Equals(entry.field, "Model name:", StringComparison.OrdinalIgnoreCase))
                {
                    result.model = entry.data;
                }
                else if (string.Equals(entry.field, "CPU(s):", StringComparison.OrdinalIgnoreCase))
                {
                    result.cpuCount = int.Parse(entry.data);
                }
            }

            // Get CPU usage
            result.usage = new CPUUsage()
            {
                lastSecond = GetCpuUsage(1),
                last5Seconds = GetCpuUsage(5),
                lastMinute = GetCpuUsage(60),
                last5Minutes = GetCpuUsage(300),
            };

            // Done
            return result;
        }

        public static DiskReport GetDisk()
        {
            // df -B1 --output=size,used,avail /
            var lines = OSUtils.GetLines(OSUtils.RunWithOutput("df", "-B1", "--output=size,used,avail", "/"));
            if (lines == null || lines.Count < 2)
                return new DiskReport();

            var parts = lines[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
                return new DiskReport();

            var bytesTotal = long.Parse(parts[0]);
            var bytesUsed = long.Parse(parts[1]);
            var bytesAvailable = long.Parse(parts[2]);
            return new DiskReport()
            {
                bytesTotal = bytesTotal,
                bytesUsed = bytesUsed,
                bytesAvailable = bytesAvailable,
                freePercentage = Math.Round(100 * bytesAvailable / (double)bytesTotal, 2)
            };
        }

        private static void Collect1SecondData()
        {
            // CPU usage
            // cpu 79242 0    74306  842486413 756859 6140 67701
            // cpu user  nice system idle      iowait irq  softirq 
            var str = File.ReadAllLines("/proc/stat");
            var line = str.FirstOrDefault(item => item.ToLower().StartsWith("cpu "));
            if (line == null)
                return;
            var parts = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 8)
                return;

            lock (_lock)
            {
                // Calculate totals
                var longParts = parts.Skip(1).Take(7).Select(item => long.Parse(item)).ToArray();
                cpu1SecMeasurements.Add(new CpuMeasurement()
                {
                    totalTime = longParts.Sum(),
                    idleTime = longParts[3],
                    ioWaitTime = longParts[4],
                });

                while (cpu1SecMeasurements.Count > 300)
                    cpu1SecMeasurements.RemoveAt(0);
            }
        }

        public static DateTimeReport GetDateTime()
        {
            string sTimeZone = OSUtils.GetLines(OSUtils.RunWithOutput("timedatectl", "show")).FirstOrDefault(item => item.ToLower().StartsWith("timezone="));
            if (!string.IsNullOrWhiteSpace(sTimeZone))
                sTimeZone = sTimeZone.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1];
            return new DateTimeReport()
            {
                utc = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                local = DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture),
                timeZone = sTimeZone,
                uptime = GetUptime(),
            };
        }

        public static SystemStatsReport GetReport()
        {
            return new SystemStatsReport()
            {
                dateTime = GetDateTime(),
                cpuReport = GetCpuReport(),
                memory = GetMem(),
                disk = GetDisk(),
                temperature = GetTemps(),
                wiFi = WiFiManager.GetWiFiStatus(),
            };
        }

        #region Background service: heartbeat

        public class BackgroundTelemetry(ILogger<BackgroundTelemetry> logger) : BackgroundService
        {
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                logger.LogInformation("Background telemetry service started");

                var sw = Stopwatch.StartNew();
                var nextSec = 1000;

                // Signal that the background task has started
                await Task.Delay(1000);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        if (Global.IsDevelopment)
                        {
                            await Task.Delay(1000);
                        }
                        else
                        {
                            await Task.Delay(800);
                            while (sw.ElapsedMilliseconds < nextSec)
                                Thread.Sleep(10);
                            nextSec += 1000;
                            SystemStats.Collect1SecondData();
                        }
                    } 
                    catch (Exception ex)
                    {
                        logger.LogError($"Background telemetry error: {ex}");
                        await Task.Delay(1000);
                    }
                }

                logger.LogInformation("Background telemetry service stopped");
            }
        }

        #endregion


    }
}
