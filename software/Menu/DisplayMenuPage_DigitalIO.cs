using System;

namespace cog1app
{
    public class DisplayMenuPage_DigitalIO : DisplayMenuPage
    {
        public override void Update()
        {
            var canvas = new DisplayCanvas();
            var font = DisplayCanvas.Font_6x8;

            // Read inputs
            bool[] inputs = new bool[4];
            for (var i = 0; i < inputs.Length; i++)
                inputs[i] = IOManager.GetDigitalInput(i + 1);

            // Read outputs
            bool[] outputs = new bool[4];
            for (var i = 0; i < outputs.Length; i++)
                outputs[i] = IOManager.GetDigitalOutput(i + 1);

            // Title
            canvas.DrawTitle(0, "D-IO", 40, "IN", 90, "OUT");
            canvas.DrawPlusSign();

            // Channel numbers
            canvas.Draw1234(9);

            // Draw inputs
            canvas.DrawSwitch(40, 16, inputs[0]);
            canvas.DrawSwitch(40, 28, inputs[1]);
            canvas.DrawSwitch(40, 40, inputs[2]);
            canvas.DrawSwitch(40, 52, inputs[3]);

            // Draw outputs
            canvas.DrawSwitch(90, 16, outputs[0]);
            canvas.DrawSwitch(90, 28, outputs[1]);
            canvas.DrawSwitch(90, 40, outputs[2]);
            canvas.DrawSwitch(90, 52, outputs[3]);

            canvas.ToDisplay();
        }

        public override DisplayMenuAction EncoderButtonDown(out DisplayMenuPage newPage)
        {
            newPage = new DisplayMenuPage_DO_Control();
            return DisplayMenuAction.Push;
        }
    }
}
