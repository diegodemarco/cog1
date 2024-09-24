namespace cog1app
{
    public class DisplayMenuPage_DO_Control : DisplayMenuPage
    {
        private int selected = 0;

        public override void Update()
        {
            // Read outputs
            bool[] outputs = new bool[4];
            for (var i = 0; i < outputs.Length; i++)
                outputs[i] = IOManager.GetDigitalOutput(i + 1);

            var canvas = new DisplayCanvas();
            canvas.DrawTitle("DO-Control");
            canvas.DrawBackArrow(selected == 0);
            canvas.Draw1234(9);

            // Draw outputs
            canvas.DrawSwitch(70, 16, outputs[0]);
            canvas.DrawSwitch(70, 28, outputs[1]);
            canvas.DrawSwitch(70, 40, outputs[2]);
            canvas.DrawSwitch(70, 52, outputs[3]);

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
                IOManager.SetDigitalOutput(selected, !IOManager.GetDigitalOutput(selected));
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
