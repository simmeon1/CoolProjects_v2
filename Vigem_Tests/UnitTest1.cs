using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigem_ClassLibrary;
using WindowsPixelReader;

namespace Vigem_Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly PixelReader pixelReader = new();
        private readonly Ds4ControllerUser controllerUser = new(new Ds4Controller(), 500);
        [TestMethod]
        public async Task TestHaveCjSwimInCircles()
        {
            controllerUser.Connect();
            await controllerUser.PressButton(ButtonMappings.Options);
            controllerUser.HoldDPad(DPadMappings.West);
            controllerUser.HoldButton(ButtonMappings.Cross);
            controllerUser.HoldStick(AxisMappings.LeftThumbX, byte.MaxValue);
            while (true)
            {
                await Task.Delay(2000);
                Color cursorColor = GetPixelDetails(pixelReader.GetCursorLocation(), "Cursor");
                Color hardcodedColor = GetPixelDetails(new Point(2239, 889), "Hardcoded");
                
                if (hardcodedColor.GetBrightness() < 0.5) break;
            }
            await controllerUser.PressButton(ButtonMappings.Options);
            controllerUser.Disconnect();
        }

        private Color GetPixelDetails(Point pos, string describer)
        {
            Color color = pixelReader.GetColorAtLocation(pos);
            float brightness = color.GetBrightness();
            Debug.WriteLine(describer + " - " + pos.X + ", " + pos.Y + ", " + brightness);
            return color;
        }
    }
}