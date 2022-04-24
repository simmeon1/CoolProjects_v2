using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Common_ClassLibrary;
using Common_ClassLibrary.Interfaces;

namespace AutoInput
{
    public class ActionPlayer
    {
        private readonly IDelayer delayer;
        private readonly INativeMethods nativeMethods;
        private readonly DualshockControllerWrapper controller;

        public ActionPlayer(IDelayer delayer, INativeMethods nativeMethods, DualshockControllerWrapper controller)
        {
            this.delayer = delayer;
            this.nativeMethods = nativeMethods;
            this.controller = controller;
        }

        public async Task PlayAction(Action action)
        {
            if (!action.Enabled) await delayer.GetCompletedTask();

            ActionType type = action.Type;
            string[] args = action.Arguments;
            if (type == ActionType.Wait) await PlayWaitAction(args);
            else if (type == ActionType.WaitUntilPixelBrightnessIsInRange)
                await PlayWaitUntilPixelBrightnessIsInRangeAction(args);
            else if (type == ActionType.SetStates) await PlaySetStateAction(args);
        }

        private async Task PlayWaitUntilPixelBrightnessIsInRangeAction(string[] args)
        {
            int x = int.Parse(args[0]);
            int y = int.Parse(args[1]);
            double minBrightness = double.Parse(args[2]);
            double maxBrightness = double.Parse(args[3]);

            Point location = new(x, y);
            while (true)
            {
                Color color = nativeMethods.GetColorAtLocation(location);
                float brightness = color.GetBrightness();
                if (brightness >= minBrightness && brightness <= maxBrightness) break;
            }

            await delayer.GetCompletedTask();
        }

        private async Task PlaySetStateAction(string[] args)
        {
            string stateJson = args[0];
            List<ControllerState> states = ControllerState.FromJsonArray(stateJson);

            List<double> timeDiffs = new();
            for (int i = 0; i < states.Count - 1; i++) timeDiffs.Add(states[i + 1].TIMESTAMP - states[i].TIMESTAMP);
            timeDiffs = timeDiffs.OrderByDescending(t => t).ToList();
            //Min should be around 25
            double maxDiff = timeDiffs.Count == 0 ? 0 : timeDiffs.First();
            double minDiff = timeDiffs.Count == 0 ? 0 : timeDiffs.Last();

            double firstTimestamp = states[0].TIMESTAMP;
            foreach (ControllerState state in states) state.TIMESTAMP -= firstTimestamp;
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
                // Console.WriteLine(watch.ElapsedMilliseconds);
                // long watchElapsedMilliseconds = watch.ElapsedMilliseconds;
                // double timeToWait = timeDiffBetweenStates - watchElapsedMilliseconds;
                // await delayer.Delay((int) timeToWait);
                double timeToWait = Math.Max(0, nextControllerState.TIMESTAMP - watch.ElapsedMilliseconds);
                if (timeToWait > 0) await delayer.Delay((int) timeToWait);
                // while (watch.ElapsedMilliseconds < nextControllerState.TIMESTAMP)
                // {
                // }
            }
            await delayer.GetCompletedTask();
        }

        private async Task PlayWaitAction(string[] args)
        {
            int delay = int.Parse(args[0]);
            await delayer.Delay(delay);
        }
    }
}