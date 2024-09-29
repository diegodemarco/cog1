using System;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_System : DisplayMenuPage
    {
        public override void Update()
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_6x8;

            // Title
            canvas.DrawTitle("SYSTEM");
            canvas.DrawPlusSign();

            // Options
            canvas.DrawText(0, 30, font, "    Press button");
            canvas.DrawText(0, 40, font, "    for options.");

            canvas.ToDisplay();
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = new DisplayMenuPage_System_Control();
            return DisplayMenuAction.Push;
        }
    }
}
