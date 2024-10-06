using System;
using cog1.Hardware;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_DigitalIO : DisplayMenuPage
    {
        const int update_seconds = 2;       // Update every 2 seconds

        int next_update_seconds = update_seconds;

        public override void Update()
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_6x8;

            // Read inputs and outputs
            IOManager.ReadDI(out var di1, out var di2, out var di3, out var di4);
            IOManager.ReadDO(out var do1, out var do2, out var do3, out var do4);

            // Title
            canvas.DrawTitle(0, "D-IO", 40, "IN", 90, "OUT");
            canvas.DrawPlusSign();

            // Channel numbers
            canvas.Draw1234(9);

            // Draw inputs
            canvas.DrawSwitch(40, 16, di1);
            canvas.DrawSwitch(40, 28, di2);
            canvas.DrawSwitch(40, 40, di3);
            canvas.DrawSwitch(40, 52, di4);

            // Draw outputs
            canvas.DrawSwitch(90, 16, do1);
            canvas.DrawSwitch(90, 28, do2);
            canvas.DrawSwitch(90, 40, do3);
            canvas.DrawSwitch(90, 52, do4);

            canvas.ToDisplay();
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = new DisplayMenuPage_DO_Control();
            return DisplayMenuAction.Push;
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
