using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Common_ClassLibrary;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace AutoInput
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            DoManualTest();


            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AutoInput());
        }

        private static void DoManualTest()
        {
            string file = File.ReadAllText(@"C:\Users\simme\OneDrive\Desktop\testMarioNes2.json");
            List<Action> actions = file.DeserializeObject<List<Action>>();
            List<ControllerState> states = actions[1].Arguments[0].DeserializeObject<List<ControllerState>>();
            double firstTimestamp = states[0].TIMESTAMP;
            foreach (ControllerState state in states) state.TIMESTAMP -= firstTimestamp;

            ViGEmClient client = new();
            IDualShock4Controller controller = client.CreateDualShock4Controller();
            controller.AutoSubmitReport = false;
            controller.Connect();

            while (true)
            {
                ControllerState lastState = null;
                Thread.Sleep(1000);
                Stopwatch timer = new();
                timer.Start();
                for (int i = 0; i < states.Count; i++)
                {
                    ControllerState controllerState = states[i];
                    if (timer.ElapsedMilliseconds < controllerState.TIMESTAMP)
                    {
                        i--;
                        continue;
                        // Thread.Sleep((int) (controllerState.TIMESTAMP - timer.ElapsedMilliseconds));
                    }

                    if (lastState != null && lastState.A0 != controllerState.A0)
                        controller.SetAxisValue(0, controllerState.A0);
                    if (lastState != null && lastState.A1 != controllerState.A1)
                        controller.SetAxisValue(1, controllerState.A1);
                    if (lastState != null && lastState.A2 != controllerState.A2)
                        controller.SetAxisValue(2, controllerState.A2);
                    if (lastState != null && lastState.A3 != controllerState.A3)
                        controller.SetAxisValue(3, controllerState.A3);
                    if (lastState != null && lastState.B0 != controllerState.B0)
                        controller.SetButtonState(DualShock4Button.Cross, controllerState.B0);
                    if (lastState != null && lastState.B1 != controllerState.B1)
                        controller.SetButtonState(DualShock4Button.Circle, controllerState.B1);
                    if (lastState != null && lastState.B2 != controllerState.B2)
                        controller.SetButtonState(DualShock4Button.Square, controllerState.B2);
                    if (lastState != null && lastState.B3 != controllerState.B3)
                        controller.SetButtonState(DualShock4Button.Triangle, controllerState.B3);
                    if (lastState != null && lastState.B4 != controllerState.B4)
                        controller.SetButtonState(DualShock4Button.ShoulderLeft, controllerState.B4);
                    if (lastState != null && lastState.B5 != controllerState.B5)
                        controller.SetButtonState(DualShock4Button.ShoulderRight, controllerState.B5);
//if (lastState != null && lastState.B6 != controllerState.B6)  //controller.SetSliderValue(DualShock4Slider.LeftTrigger,GetSliderValue(controllerState.B6));
//if (lastState != null && lastState.B7 != controllerState.B7)  //controller.SetSliderValue(DualShock4Slider.RightTrigger,GetSliderValue(controllerState.B7));
                    if (lastState != null && lastState.B8 != controllerState.B8)
                        controller.SetButtonState(DualShock4Button.Share, controllerState.B8);
                    if (lastState != null && lastState.B9 != controllerState.B9)
                        controller.SetButtonState(DualShock4Button.Options, controllerState.B9);
                    if (lastState != null && lastState.B10 != controllerState.B10)
                        controller.SetButtonState(DualShock4Button.ThumbLeft, controllerState.B10);
                    if (lastState != null && lastState.B11 != controllerState.B11)
                        controller.SetButtonState(DualShock4Button.ThumbRight, controllerState.B11);
                    if (controllerState.B12) controller.SetDPadDirection(DualShock4DPadDirection.North);
                    if (controllerState.B13) controller.SetDPadDirection(DualShock4DPadDirection.South);
                    if (controllerState.B14) controller.SetDPadDirection(DualShock4DPadDirection.West);
                    if (controllerState.B15) controller.SetDPadDirection(DualShock4DPadDirection.East);
                    if (!controllerState.B12 && !controllerState.B13 && !controllerState.B14 && !controllerState.B15)
                        controller.SetDPadDirection(DualShock4DPadDirection.None);
                    // Controller.SetButtonState(DualShock4Button.ps, state.b16);
                    // Controller.SetButtonState(DualShock4Button.touch, state.b17);
                    controller.SubmitReport();
                    lastState = controllerState;
                }
            }
        }
    }
}