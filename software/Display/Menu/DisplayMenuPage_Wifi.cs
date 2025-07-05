using cog1.Hardware;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_WiFi : DisplayMenuPage
    {
        public override void Update()
        {
            _Update(true);
        }

        public void _Update(bool useHourGlass)
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_6x8;

            canvas.DrawTitle("WI-FI");
            if (useHourGlass)
            {
                canvas.DrawHourglass();
                canvas.ToDisplay();
            }

            var wifi = WiFiManager.GetStatus();
            var status = wifi.isConnected ? "Connected" : "Disconnected";
            var ssid = wifi.isConnected ? wifi.ssid : "-";
            var ipv4 = wifi.isConnected ? wifi.ipConfiguration.ipv4 : "-";
            var band = wifi.isConnected ? wifi.frequency.ToString() + " MHz" : "-";
            var rssi = wifi.isConnected ? wifi.rssi.ToString() + " dBm" : "-";

            canvas.DrawText(0, 16, font, $"Status" + status.PadLeft(21 - 6));
            canvas.DrawText(0, 24, font, $"SSID" + ssid.PadLeft(21 - 4));
            canvas.DrawText(0, 32, font, $"IP" + (ipv4 + "/" + wifi.ipConfiguration.netMask.ToString()).PadLeft(21 - 2));
            canvas.DrawText(0, 40, font, "Band" + band.PadLeft(21 - 4));
            canvas.DrawText(0, 48, font, $"RSSI" + rssi.PadLeft(21 - 4));

            //
            canvas.ToDisplay();

        }

        public override void TickMinute()
        {
            _Update(false);
        }

    }
}
