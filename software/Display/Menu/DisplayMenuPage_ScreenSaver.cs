using System;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_ScreenSaver : DisplayMenuPage
    {
        private Random rnd = new Random();

        public override void Update()
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_8x12;
            var str = DateTime.Now.ToString("HH':'mm");

            int x = (int)rnd.NextInt64(canvas.Width - str.Length * font[0].Width + 1);
            int y = (int)rnd.NextInt64(canvas.Height - font[0].Height + 1);
            canvas.DrawText(x, y, font, str);
            //
            canvas.ToDisplay();
        }

        public override void TickMinute()
        {
            Update();
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.NextPage;
        }

        public override DisplayMenuAction EncoderButtonUp(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.None;
        }

        public override DisplayMenuAction EncoderRight(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.NextPage;
        }

        public override DisplayMenuAction EncoderLeft(out DisplayMenuPage newPage)
        {
            newPage = null;
            return DisplayMenuAction.NextPage;
        }

    }
}
