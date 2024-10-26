using cog1.Hardware;

namespace cog1.Display.Menu
{
    public class DisplayMenuPage_DO_Control : DisplayMenuPage
    {
        private int selected = 0;

        public override void Update()
        {
            // Read outputs
            IOManager.ReadDO(out var do1, out var do2, out var do3, out var do4);

            var canvas = new DisplayCanvas();
            canvas.DrawTitle("DO-Control");
            canvas.DrawBackArrow(selected == 0);
            canvas.Draw1234(9);

            // Draw outputs
            canvas.DrawSwitch(70, 16, do1);
            canvas.DrawSwitch(70, 28, do2);
            canvas.DrawSwitch(70, 40, do3);
            canvas.DrawSwitch(70, 52, do4);

            // Reverse as needed
            if (selected == 1)
                canvas.Reverse(0, 16, 127, 16 + 11);
            if (selected == 2)
                canvas.Reverse(0, 28, 127, 28 + 11);
            if (selected == 3)
                canvas.Reverse(0, 40, 127, 40 + 11);
            if (selected == 4)
                canvas.Reverse(0, 52, 127, 52 + 11);

            // Draw
            canvas.ToDisplay();
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = null;
            if (selected == 0)
            {
                return DisplayMenuAction.Pop;
            }
            else
            {
                IOManager.ReadDO(out var do1, out var do2, out var do3, out var do4);
                var doArr = new bool[] { do1, do2, do3, do4 };
                IOManager.SetVariableValue(selected + IOManager.DO1_VARIABLE_ID - 1, doArr[selected - 1] ? 0 : 1);
                Update();
                return DisplayMenuAction.None;
            }

        }

        public override DisplayMenuAction EncoderRight(out DisplayMenuPage newPage)
        {
            selected++;
            if (selected > 4)
                selected = 0;
            Update();
            newPage = null;
            return DisplayMenuAction.None;
        }

        public override DisplayMenuAction EncoderLeft(out DisplayMenuPage newPage)
        {
            selected--;
            if (selected < 0)
                selected = 4;
            Update();
            newPage = null;
            return DisplayMenuAction.None;
        }

    }
}
