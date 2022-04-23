using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using SharpDX.DirectInput;
using ViGEm_Common;

namespace ViGEm_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WindowsNativeMethods nativeMethods = new();
            DualshockControllerWrapper controller = new();
            Joystick controllerHandle = InitialiseControllerAndGetHandle(controller);
            controllerHandle.Acquire();
            string statesJson = File.ReadAllText(@"C:\Users\simme\OneDrive\Desktop\controller_path.json");
            List<ControllerState> states = ControllerState.FromJsonArray(statesJson);
            double firstTimestamp = states[0].TIMESTAMP;

            List<double> timeDiffs = new();
            for (int i = 0; i < states.Count - 1; i++) timeDiffs.Add(states[i + 1].TIMESTAMP - states[i].TIMESTAMP);
            // for (int i = 1; i < states.Count; i++)
            // {
            //     var lastState = states[i - 1];
            //     var state = states[i];
            //     if (state.A0 == lastState.A0 &&
            //         state.A1 == lastState.A1 &&
            //         state.A2 == lastState.A2 &&
            //         state.A3 == lastState.A3 &&
            //         state.B0 == lastState.B0 &&
            //         state.B1 == lastState.B1 &&
            //         state.B2 == lastState.B2 &&
            //         state.B3 == lastState.B3 &&
            //         state.B4 == lastState.B4 &&
            //         state.B5 == lastState.B5 &&
            //         state.B6 == lastState.B6 &&
            //         state.B7 == lastState.B7 &&
            //         state.B8 == lastState.B8 &&
            //         state.B9 == lastState.B9 &&
            //         state.B10 == lastState.B10 &&
            //         state.B11 == lastState.B11 &&
            //         state.B12 == lastState.B12 &&
            //         state.B13 == lastState.B13 &&
            //         state.B14 == lastState.B14 &&
            //         state.B15 == lastState.B15 &&
            //         state.B16 == lastState.B16 &&
            //         state.B17 == lastState.B17)
            //     {
            //         states.RemoveAt(i);
            //         i--;
            //     }
            // }

            foreach (ControllerState state in states) state.TIMESTAMP -= firstTimestamp;
            timeDiffs = timeDiffs.OrderByDescending(t => t).ToList();
            //Min should be around 25
            double maxDiff = timeDiffs.First();
            double minDiff = timeDiffs.Last();

            while (true)
            {
                Stopwatch watch = new();
                watch.Start();
                for (int i = 0; i < states.Count; i++)
                {
                    ControllerState controllerState = states[i];
                    ControllerState nextControllerState = i == states.Count - 1 ? states[i] : states[i + 1];
                    // double timeDiffBetweenStates = nextControllerState.TIMESTAMP - controllerState.TIMESTAMP;
                    // string deviceState = controllerHandle.GetCurrentState().ToString();
                    controller.SetState(controllerState);
                    // watch.Restart();
                    //WaitUntilStateIsUpdate(deviceState, controllerHandle);
                    Console.WriteLine(watch.ElapsedMilliseconds);
                    // long watchElapsedMilliseconds = watch.ElapsedMilliseconds;
                    // double timeToWait = timeDiffBetweenStates - watchElapsedMilliseconds;
                    // await Task.Delay((int) timeToWait).ConfigureAwait(false);
                    while (watch.ElapsedMilliseconds < nextControllerState.TIMESTAMP) { }
                }

                // Point pos = new(4109, 324);
                Point pos = new(2125, 570);

                // while (true)
                // {
                //     if (nativeMethods.GetColorAtLocation(pos).GetBrightness() == 0) break;
                //     await Task.Delay(1000).ConfigureAwait(false);
                //     // Thread.Sleep(1000);
                // }
                //
                // while (true)
                // {
                //     if (nativeMethods.GetColorAtLocation(pos).GetBrightness() == 0) continue;
                //     await Task.Delay(3000).ConfigureAwait(false);
                //     // Thread.Sleep(3000);
                //     break;
                // }
            }

            var x = 1;

            // List<string> deviceStates = new();
            // Stopwatch watch = new();
            // int sleep = 1;
            // for (int i = 0; i < 1000; i++)
            // {
            //     string deviceState = controllerHandle.GetCurrentState().ToString();
            //     deviceStates.Add(deviceState);
            //     controller.SetButtonState(DualShock4Button.Cross, i % 2 == 0);
            //     await Task.Delay(sleep);
            //     watch.Restart();
            //     WaitUntilStateIsUpdate(deviceState, controllerHandle);
            //     long watchElapsedMilliseconds = watch.ElapsedMilliseconds;
            // }

            var y = 1;
        }

        private static void WaitUntilStateIsUpdate(string deviceState, Joystick controllerHandle)
        {
            while (deviceState.Equals(controllerHandle.GetCurrentState().ToString()))
            {
                var x = 1;
            }
        }

        private static Joystick InitialiseControllerAndGetHandle(DualshockControllerWrapper controller)
        {
            DirectInput directInput = new();
            IList<DeviceInstance> firstDevices = directInput.GetDevices();
            controller.StartController();
            while (true)
            {
                IList<DeviceInstance> secondDevices = directInput.GetDevices();
                foreach (DeviceInstance device in secondDevices)
                {
                    if (!firstDevices.Any(d => d.InstanceGuid.Equals(device.InstanceGuid)))
                    {
                        return new Joystick(directInput, device.InstanceGuid);
                    }
                }
            }
        }
    }
}