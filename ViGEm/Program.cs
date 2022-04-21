using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static MouseOperations;

namespace ViGEm
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ViGEmClient client = new();


            WindowsNativeMethods methods = new();
            // while (true)
            // {
            //     // Point pos = methods.GetCursorPosition();
            //     Point pos = new(2663, 43);
            //     Console.WriteLine($"{pos.X}, {pos.Y}, {methods.GetColorAt(pos).GetBrightness()}");
            //     await Task.Delay(1000);
            // }

            IDualShock4Controller controller = client.CreateDualShock4Controller();
            controller.Connect();
            DualshockControllerWrapper c = new(controller);

            while (true)
            {
                controller.SetButtonState(DualShock4Button.Options, System.Windows.Input.);
                //lx
                // Controller.SetAxisValue(0, CalculateThumbPositionFromStateValue(state.A0));
                // //ly
                // Controller.SetAxisValue(1, CalculateThumbPositionFromStateValue(state.A1));
                // //rx
                // Controller.SetAxisValue(2, CalculateThumbPositionFromStateValue(state.A2));
                // //ry
                // Controller.SetAxisValue(3, CalculateThumbPositionFromStateValue(state.A3));
                // Controller.SetButtonState(DualShock4Button.Cross, state.B0);
                // Controller.SetButtonState(DualShock4Button.Circle, state.B1);
                // Controller.SetButtonState(DualShock4Button.Square, state.B2);
                // Controller.SetButtonState(DualShock4Button.Triangle, state.B3);
                // Controller.SetButtonState(DualShock4Button.ShoulderLeft, state.B4);
                // Controller.SetButtonState(DualShock4Button.ShoulderRight, state.B5);
                // Controller.SetButtonState(DualShock4Button.TriggerLeft, state.B6);
                // Controller.SetButtonState(DualShock4Button.TriggerRight, state.B7);
                // Controller.SetButtonState(DualShock4Button.Share, state.B8);
                // Controller.SetButtonState(DualShock4Button.Options, state.B9);
                // Controller.SetButtonState(DualShock4Button.ThumbLeft, state.B10);
                // Controller.SetButtonState(DualShock4Button.ThumbRight, state.B11);
            }
            
            
            string json = File.ReadAllText(@"C:\Users\simme\OneDrive\Desktop\controller_path.json");
            List<HtmlControllerState> states = HtmlControllerState.FromJsonArray(json);
            //states.RemoveRange(0, 10);
            
            Stopwatch stopwatch = new();
            await Task.Delay(1000);
            
            for (int i = 0; i < states.Count; i++)
            {
                HtmlControllerState state = states[i];
                c.SetStateFromHtmlControllerState(state);
                HtmlControllerState nextState = i == states.Count - 1 ? state : states[i + 1];
                double timeDiff = nextState.TIMESTAMP - state.TIMESTAMP;
                stopwatch.Restart();
                // await Task.Delay(timeDiff);
                while (stopwatch.ElapsedMilliseconds <= timeDiff) { }
            }
        }

        private static async Task SekiroFarm(DualshockControllerWrapper c)
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
