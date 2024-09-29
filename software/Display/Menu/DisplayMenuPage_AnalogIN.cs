using System.Runtime.CompilerServices;
using cog1.Hardware;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_AnalogIN : DisplayMenuPage
    {
        const int update_seconds = 2;       // Update every 2 seconds

        private const string icon_up = "▲";
        private const string icon_down = "▼";
        int next_update_seconds = update_seconds;

        public override void Update()
        {
            const double v_limit_low = 1;
            const double v_limit_high = 10;
            const double ma_limit_low = 4;
            const double ma_limit_high = 20;

            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_8x12;

            IOManager.Read020mA(out double ana1, out double ana2, out double ana3, out double ana4);
            IOManager.Read010V(out double anv1, out double anv2, out double anv3, out double anv4);

            canvas.DrawTitle(0, "A-IN", 51, "V", 94, "mA");
            canvas.Draw1234(9);

            // Volts
            canvas.DrawText(37, 16, font, string.Format("{0,4:F1}", anv1) + GetSymbol(anv1, v_limit_low, v_limit_high));
            canvas.DrawText(37, 28, font, string.Format("{0,4:F1}", anv2) + GetSymbol(anv2, v_limit_low, v_limit_high));
            canvas.DrawText(37, 40, font, string.Format("{0,4:F1}", anv3) + GetSymbol(anv3, v_limit_low, v_limit_high));
            canvas.DrawText(37, 52, font, string.Format("{0,4:F1}", anv4) + GetSymbol(anv4, v_limit_low, v_limit_high));

            // mA
            canvas.DrawText(87, 16, font, string.Format("{0,4:F1}", ana1) + GetSymbol(ana1, ma_limit_low, ma_limit_high));
            canvas.DrawText(87, 28, font, string.Format("{0,4:F1}", ana2) + GetSymbol(ana2, ma_limit_low, ma_limit_high));
            canvas.DrawText(87, 40, font, string.Format("{0,4:F1}", ana3) + GetSymbol(ana3, ma_limit_low, ma_limit_high));
            canvas.DrawText(87, 52, font, string.Format("{0,4:F1}", ana4) + GetSymbol(ana4, ma_limit_low, ma_limit_high));

            canvas.ToDisplay();
        }

        private static string GetSymbol(double value, double limit_low, double limit_high)
        {
            if (value < limit_low)
                return icon_down;
            if (value > limit_high)
                return icon_up;
            return "";
        }

        public override void TickSecond()
        {
            base.TickSecond();
            next_update_seconds--;
            if (next_update_seconds <= 0)
            {
                next_update_seconds = update_seconds;
                Update();
            }
        }

    }
}
