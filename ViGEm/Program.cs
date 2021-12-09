using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using static MouseOperations;

namespace ViGEm
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ViGEmClient client = new();

            IDualShock4Controller controller = client.CreateDualShock4Controller();
            controller.Connect();
            DualshockControllerWrapper c = new(controller);

            while (true)
            {

                //x = 2856
                //y = 956
                //color = "ffc748ce"
                //SetCursorPosition(2865, 955);
                //MousePoint mouse = GetCursorPosition();
                //Color cc = GetColorAt(new Point(mouse.X, mouse.Y));

                while (true)
                {
                    try
                    {
                        Color colour = GetColorAt(new Point(2856, 956));
                        if (colour.Name.StartsWith("ffc")) break;
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        //Repeat
                    }
                }

                //Up the stairs and wall
                await c.PushStickUp(DualShock4Axis.RightThumbX, 800);
                await c.PushStickDown(DualShock4Axis.LeftThumbY, 2000);

                //Hug wall
                await c.PressButton(DualShock4Button.Square);

                //Go to end
                await c.PushStickDown(DualShock4Axis.LeftThumbX, 2000);

                //Wait
                await Task.Delay(4000);

                //Kill
                await c.PressButton(DualShock4Button.ShoulderRight, delayAfterRelease: 4000);

                //Get gold
                await c.PressButton(DualShock4Button.Square, delayBeforeRelease: 1500);

                //Reset
                await c.PressButton(DualShock4Button.Options);
                await c.PressButton(DualShock4Button.ShoulderRight, 100);
                await c.PressDpad(DualShock4DPadDirection.North, 100);
                await c.PressButton(DualShock4Button.Cross);
                await c.PressButton(DualShock4Button.Cross);
                await c.PressDpad(DualShock4DPadDirection.North, 100);
                await c.PressButton(DualShock4Button.Cross);
            }
        }
    }
}
