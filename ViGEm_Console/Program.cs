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
            foreach (ControllerState state in states) state.TIMESTAMP -= firstTimestamp;
            timeDiffs = timeDiffs.OrderByDescending(t => t).ToList();
            //Min should be around 25
            double maxDiff = timeDiffs.First();
            double minDiff = timeDiffs.Last();

            while (true)
            {
                Stopwatch watch = new();
                for (int i = 0; i < states.Count; i++)
                {
                    ControllerState controllerState = states[i];
                    ControllerState nextControllerState = i == states.Count - 1 ? states[i] : states[i + 1];
                    double timeDiffBetweenStates = nextControllerState.TIMESTAMP - controllerState.TIMESTAMP;
                    string deviceState = controllerHandle.GetCurrentState().ToString();
                    controller.SetState(controllerState);
                    watch.Restart();
                    WaitUntilStateIsUpdate(deviceState, controllerHandle);
                    long watchElapsedMilliseconds = watch.ElapsedMilliseconds;
                    double timeToWait = timeDiffBetweenStates - watchElapsedMilliseconds;
                    await Task.Delay((int) timeToWait).ConfigureAwait(false);
                }

                // Point pos = new(4109, 324);
                Point pos = new(2125, 570);

                while (true)
                {
                    if (nativeMethods.GetColorAtLocation(pos).GetBrightness() == 0) break;
                    await Task.Delay(1000).ConfigureAwait(false);
                    // Thread.Sleep(1000);
                }

                while (true)
                {
                    if (nativeMethods.GetColorAtLocation(pos).GetBrightness() == 0) continue;
                    await Task.Delay(3000).ConfigureAwait(false);
                    // Thread.Sleep(3000);
                    break;
                }
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