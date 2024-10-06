using System;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_Confirm_Warning : DisplayMenuPage
    {
        private int selected = 1;       // Default: cancel
        private readonly string title;
        private readonly string option1;
        private readonly Action action;

        public DisplayMenuPage_Confirm_Warning(string title, string option1, Action action) : base()
        {
            this.title = title;
            this.option1 = option1;
            this.action = action;
        }

        public override void Update()
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_8x12;

            canvas.DrawTitle(title.ToUpper());
            canvas.DrawWarningSign();
            canvas.DrawText(2, 16, font, option1);
            canvas.DrawText(2, 28, font, "Cancel");

            if (selected == 0)
                canvas.Reverse(0, 16, canvas.Width - 1, 27);
            if (selected == 1)
                canvas.Reverse(0, 28, canvas.Width - 1, 39);

            //
            canvas.ToDisplay();
        }

        public override DisplayMenuAction EncoderRight(out DisplayMenuPage newPage)
        {
            selected++;
            if (selected > 1)
                selected = 0;
            Update();
            newPage = null;
            return DisplayMenuAction.None;
        }

        public override DisplayMenuAction EncoderLeft(out DisplayMenuPage newPage)
        {
            selected--;
            if (selected < 0)
                selected = 1;
            Update();
            newPage = null;
            return DisplayMenuAction.None;
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            if (selected == 0)
                action();
            newPage = null;
            return DisplayMenuAction.Pop;
        }

    }
}
