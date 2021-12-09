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
