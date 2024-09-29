using System;
using cog1.Hardware;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_Home : DisplayMenuPage
    {
        private bool hasCPU = false;

        public override void Update()
        {
            _Update(true);
        }

        public void _Update(bool useHourGlass)
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_6x8;

            canvas.DrawTitle("STATUS");
            if (useHourGlass)
            {
                canvas.DrawHourglass();
                canvas.ToDisplay();
            }

            var usedCPU = "------";
            var cpuUsage = SystemStats.GetCpuUsage(60);
            if (cpuUsage != null)
            {
                usedCPU = string.Format("{0,5:F1}%", 100.00 - cpuUsage.idlePercentage - cpuUsage.ioWaitPercentage);
                hasCPU = true;
            }

            var temps = SystemStats.GetTemps();
            var wifi = WiFiManager.GetWiFiStatus();
            var wifiState = wifi.isConnected ? wifi.ipv4 : "Disconnected";

            canvas.DrawText(0, 16, font, $"Date {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
            canvas.DrawText(0, 24, font, $"WiFi" + wifiState.PadLeft(21 - 4));
            canvas.DrawText(0, 32, font, $"Eth                  ");
            canvas.DrawText(0, 40, font, "Temp" + $"{(int?)temps.maxTemperatureC}ºC/{(int?)Utils.CelsiusToFahrenheit(temps.maxTemperatureC)}ºF".PadLeft(21 - 4));
            canvas.DrawText(0, 48, font, $"CPU (1 min)    {usedCPU}");
            canvas.DrawText(0, 56, font, $"Firmware" + Global.Version.PadLeft(21 - 8));

            //
            canvas.ToDisplay();

        }

        public override void TickMinute()
        {
            _Update(false);
        }

        public override void TickSecond()
        {
            // Initially try to get CPU data until we have that information
            if (!hasCPU)
            {
                if (SystemStats.GetCpuUsage(60) != null)
                {
                    hasCPU = true;
                    _Update(false);
                }
            }
        }

    }
}
