using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using Timer = System.Timers.Timer;

namespace AutoInput
{
    public static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task Main()
        {
            await DoManualTest();


            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AutoInput());
        }

        private static async Task DoManualTest()
        {
            // Stopwatch delayTimer = new();
            // // delayTimer.Start();
            // List<double> delayDiffs = new();
            // for (int i = 0; i < 20; i++)
            // {
            //     delayDiffs.Add(delayTimer.Elapsed.TotalMilliseconds);
            //     delayTimer.Restart();
            //     // while (delayTimer.Elapsed.TotalMilliseconds < 20)
            //     // {
            //     //     
            //     // }
            //     
            //     Thread.Sleep(20);
            // }
            //
            // double delayAvg = delayDiffs.Average(d => d);
            
            string file = await File.ReadAllTextAsync(@"C:\Users\simme\OneDrive\Desktop\testMarioNes7.json");
            // string file = File.ReadAllText(@"C:\Users\simme\OneDrive\Desktop\darkSoulsTest.json");
            List<Action> actions = file.DeserializeObject<List<Action>>();
            List<ControllerState> states = actions[0].Arguments[0].DeserializeObject<List<ControllerState>>();
            
            double firstTimestamp = states[0].TIMESTAMP;
            // double lowestDiff = Double.MaxValue;
            List<double> diffs = new();
            foreach (ControllerState state in states)
            {
                double diff = state.TIMESTAMP - firstTimestamp;
                Debug.WriteLine(diff);
                // if (diff < lowestDiff && diff != 0) lowestDiff = diff;
                diffs.Add(diff);
                firstTimestamp = state.TIMESTAMP;
            }

            diffs = diffs.OrderBy(d => d).ToList();
            
            // foreach (ControllerState state in states) state.TIMESTAMP -= firstTimestamp;
            // foreach (ControllerState state in states) state.TIMESTAMP = (int) (state.TIMESTAMP);

            ViGEmClient client = new();
            IDualShock4Controller controller = client.CreateDualShock4Controller();
            controller.AutoSubmitReport = false;
            controller.Connect();
            
            DirectInputUseCase directInputUseCase = new(new RealDelayer());
            Joystick handle = await directInputUseCase.GetControllerJoystick();
            handle.Acquire();

            List<double> times = new();
            while (true)
            {
                ControllerState lastState = new();
                WindowsNativeMethods methods = new();

                Point location = new Point(3307, 173);
                // Point location = new Point(2125, 570);
                
                Thread.Sleep(1000);
                // while (true)
                // {
                //     if (methods.GetColorAtLocation(location).GetBrightness() > 0.99) break;
                //     // if (methods.GetColorAtLocation(location).GetBrightness() == 0) break;
                // }
                //
                // while (true)
                // {
                //     if (methods.GetColorAtLocation(location).GetBrightness() != 0) break;
                // }
                
                
                string timesJson = times.SerializeObject(Formatting.Indented);
                times.Clear();
                // double lastDelay = 0;
                Stopwatch timer = new();
                timer.Start();
                // timer.
                for (int i = 0; i < states.Count; i++)
                {
                    ControllerState controllerState = states[i];
                    // Debug.WriteLine(diff);
                    // int diff = (int) ((int)controllerState.TIMESTAMP - (int)timer.ElapsedMilliseconds);
                    // Debug.WriteLine("start - " + diff);
                    // if (timer.ElapsedMilliseconds < controllerState.TIMESTAMP)
                    // if (controllerState.TIMESTAMP - timer.Elapsed.TotalMilliseconds > 0)
                    // {
                    //     i--;
                    //     continue;
                    //     // Thread.Sleep((int) (controllerState.TIMESTAMP - timer.ElapsedMilliseconds));
                    // }

                    // double target = (controllerState.TIMESTAMP - lastState?.TIMESTAMP ?? 0);
                    // while (timer.Elapsed.TotalMilliseconds < target)
                    
                    int bonus = 0;
                    // int bonus = (i + 1) * 40;
                    // timer.Start();
                    while (controllerState.TIMESTAMP - (timer.Elapsed.TotalMilliseconds + bonus) > 0) { }
                    // int timeToWait = (int) (controllerState.TIMESTAMP - (timer.Elapsed.TotalMilliseconds + bonus));
                    // if (timeToWait > 0) await Task.Delay(timeToWait);
                    // if (timeToWait > 0) Thread.Sleep(timeToWait);
                    // timer.Stop();
                    
                    // Debug.WriteLine("start - " + (controllerState.TIMESTAMP - timer.ElapsedMilliseconds));
                    controller.SetAxisValue(0, controllerState.A0);
                    controller.SetAxisValue(1, controllerState.A1);
                    controller.SetAxisValue(2, controllerState.A2);
                    controller.SetAxisValue(3, controllerState.A3);
                    controller.SetButtonState(DualShock4Button.Cross, controllerState.B0);
                    controller.SetButtonState(DualShock4Button.Circle, controllerState.B1);
                    controller.SetButtonState(DualShock4Button.Square, controllerState.B2);
                    controller.SetButtonState(DualShock4Button.Triangle, controllerState.B3);
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, controllerState.B4);
                    controller.SetButtonState(DualShock4Button.ShoulderRight, controllerState.B5);
//if (lastState.B6 != controllerState.B6)  //controller.SetSliderValue(DualShock4Slider.LeftTrigger,GetSliderValue(controllerState.B6));
//if (lastState.B7 != controllerState.B7)  //controller.SetSliderValue(DualShock4Slider.RightTrigger,GetSliderValue(controllerState.B7));
                    controller.SetButtonState(DualShock4Button.Share, controllerState.B8);
                    controller.SetButtonState(DualShock4Button.Options, controllerState.B9);
                    controller.SetButtonState(DualShock4Button.ThumbLeft, controllerState.B10);
                    controller.SetButtonState(DualShock4Button.ThumbRight, controllerState.B11);
                    
                    if (controllerState.B12) controller.SetDPadDirection(DualShock4DPadDirection.North);
                    else if (controllerState.B13) controller.SetDPadDirection(DualShock4DPadDirection.South);
                    else if (controllerState.B14) controller.SetDPadDirection(DualShock4DPadDirection.West);
                    else if (controllerState.B15) controller.SetDPadDirection(DualShock4DPadDirection.East);
                    else controller.SetDPadDirection(DualShock4DPadDirection.None);
                    // Controller.SetButtonState(DualShock4Button.ps, state.b16);
                    // Controller.SetButtonState(DualShock4Button.touch, state.b17);
                    // lastDelay = timer.Elapsed.TotalMilliseconds - controllerState.TIMESTAMP;
                    times.Add(timer.Elapsed.TotalMilliseconds);

                    // string handleState = handle.GetCurrentState().ToString();
                    controller.SubmitReport();
                    // timer.Stop();
                    // while (handleState.Equals(handle.GetCurrentState().ToString())) { }
                    // timer.Start();
                    // Debug.WriteLine("end - " + (controllerState.TIMESTAMP - timer.ElapsedMilliseconds));
                }
            }
        }
    }
}