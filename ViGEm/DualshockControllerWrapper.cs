using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ViGEm
{
    public class DualshockControllerWrapper
    {
        public IDualShock4Controller Controller { get; set; }

        public DualshockControllerWrapper(IDualShock4Controller controller)
        {
            Controller = controller;
        }

        public void SetStateFromHtmlControllerState(HtmlControllerState state)
        {
            Controller.SetButtonState(DualShock4Button.Cross, state.B0);
            Controller.SetButtonState(DualShock4Button.Circle, state.B1);
            Controller.SetButtonState(DualShock4Button.Square, state.B2);
            Controller.SetButtonState(DualShock4Button.Triangle, state.B3);
            Controller.SetButtonState(DualShock4Button.ShoulderLeft, state.B4);
            Controller.SetButtonState(DualShock4Button.ShoulderRight, state.B5);
            Controller.SetButtonState(DualShock4Button.TriggerLeft, state.B6);
            Controller.SetButtonState(DualShock4Button.TriggerRight, state.B7);
            Controller.SetButtonState(DualShock4Button.Share, state.B8);
            Controller.SetButtonState(DualShock4Button.Options, state.B9);
            Controller.SetButtonState(DualShock4Button.ThumbLeft, state.B10);
            Controller.SetButtonState(DualShock4Button.ThumbRight, state.B11);
            
            double chunkSize = (double)2 / 255;
            List<double> chunks = new();
            for (int i = 0; i < 256; i++) chunks.Add(chunkSize * i - 1);

            Controller.SetAxisValue(DualShock4Axis.LeftThumbX, CalculateThumbPositionFromStateValue(state.A0, chunks));
            Controller.SetAxisValue(DualShock4Axis.LeftThumbY, CalculateThumbPositionFromStateValue(state.A1, chunks));
            Controller.SetAxisValue(DualShock4Axis.RightThumbX, CalculateThumbPositionFromStateValue(state.A2, chunks));
            Controller.SetAxisValue(DualShock4Axis.RightThumbY, CalculateThumbPositionFromStateValue(state.A3, chunks));
            // Controller.SetDPadDirection(DualShock4Button.up, state.b12);
            // Controller.SetButtonState(DualShock4Button.down, state.b13);
            // Controller.SetButtonState(DualShock4Button.left, state.b14);
            // Controller.SetButtonState(DualShock4Button.right, state.b15);
            // Controller.SetButtonState(DualShock4Button.ps, state.b16);
            // Controller.SetButtonState(DualShock4Button.touch, state.b17);
        }

        private static byte CalculateThumbPositionFromStateValue(double statePosition, List<double> chunks)
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                double chunk = chunks[i];
                if (statePosition <= chunk) return (byte)i;
            }
            return 128;
        }

        public async Task PressButton(DualShock4Button button, int delayBeforeRelease = 500, int delayAfterRelease = 500)
        {
            Controller.SetButtonState(button, true);
            await Task.Delay(delayBeforeRelease);
            Controller.SetButtonState(button, false);
            await Task.Delay(delayAfterRelease);
        }
        
        public async Task PushStickUp(DualShock4Axis axis, int delayBeforeRelease = 500, int delayAfterRelease = 500)
        {
            Controller.SetAxisValue(axis, 255);
            await Task.Delay(delayBeforeRelease);
            Controller.SetAxisValue(axis, 128);
            await Task.Delay(delayAfterRelease);
        }
        
        public async Task PushStickDown(DualShock4Axis axis, int delayBeforeRelease = 500, int delayAfterRelease = 500)
        {
            Controller.SetAxisValue(axis, 0);
            await Task.Delay(delayBeforeRelease);
            Controller.SetAxisValue(axis, 128);
            await Task.Delay(delayAfterRelease);
        }

        public async Task PressDpad(DualShock4DPadDirection dpad, int delayBeforeRelease = 500, int delayAfterRelease = 500)
        {
            Controller.SetDPadDirection(dpad);
            await Task.Delay(delayBeforeRelease);
            Controller.SetDPadDirection(DualShock4DPadDirection.None);
            await Task.Delay(delayAfterRelease);
        }
    }
}
