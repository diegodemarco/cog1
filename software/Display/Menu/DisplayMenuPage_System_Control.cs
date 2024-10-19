namespace cog1.Display.Menu
{
    public class DisplayMenuPage_System_Control : DisplayMenuPage
    {
        private int selected = 0;
        private const int OPTION_COUNT = 4;

        public override void Update()
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_6x8;

            canvas.DrawTitle("SYSTEM");
            canvas.DrawBackArrow(selected == 0);

            // Options
            canvas.DrawText(3, 16, font, "Reinit");
            canvas.DrawText(3, 24, font, "Reboot");
            canvas.DrawText(3, 32, font, "Shut down");

            // Highlight the selected item
            if (selected > 0)
            {
                int y = 16 + 8 * (selected - 1);
                canvas.Reverse(0, y, 127, y + 7);
            }

            // Draw
            canvas.ToDisplay();
        }

        public override DisplayMenuAction EncoderRight(out DisplayMenuPage newPage)
        {
            selected++;
            if (selected >= OPTION_COUNT)
                selected = 0;
            Update();
            newPage = null;
            return DisplayMenuAction.None;
        }

        public override DisplayMenuAction EncoderLeft(out DisplayMenuPage newPage)
        {
            selected--;
            if (selected < 0)
                selected = OPTION_COUNT - 1;
            Update();
            newPage = null;
            return DisplayMenuAction.None;
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = null;
            switch (selected)
            {
                case 0:
                    return DisplayMenuAction.Pop;
                case 1:
                    newPage = new DisplayMenuPage_Confirm_Warning("REINIT", "Reinit now", OSUtils.Reinit);
                    return DisplayMenuAction.Push;
                case 2:
                    newPage = new DisplayMenuPage_Confirm_Warning("REBOOT", "Reboot now", OSUtils.Reboot);
                    return DisplayMenuAction.Push;
                case 3:
                    newPage = new DisplayMenuPage_Confirm_Warning("SHUT DOWN", "Shut down now", OSUtils.Shutdown);
                    return DisplayMenuAction.Push;
            }
            return DisplayMenuAction.None;
        }

    }
}
