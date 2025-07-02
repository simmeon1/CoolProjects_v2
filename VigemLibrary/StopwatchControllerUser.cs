using VigemLibrary.Controllers;
using VigemLibrary.Mappings;
using VigemLibrary.SystemImplementations;

namespace VigemLibrary
{
    public class StopwatchControllerUser
    {
        private readonly IController controller;
        private readonly IStopwatch stopwatch;
        private readonly int pressLength;
        private readonly int delayAfterSet;

        // Changing away form zero could break other usages like jump rope
        public StopwatchControllerUser(IController controller, IStopwatch stopwatch, int pressLength, int delayAfterSet = 0)
        {
            this.controller = controller;
            this.stopwatch = stopwatch;
            this.pressLength = pressLength;
            this.delayAfterSet = delayAfterSet;
            this.stopwatch.Restart();
        }

        public void Connect()
        {
            controller.Connect();
        }
        
        public void Disconnect()
        {
            controller.Disconnect();
        }

        public void PressButton(ButtonMappings button, int? holdDuration = null)
        {
            HoldButton(button);
            Wait(holdDuration);
            ReleaseButton(button);
        }

        public void HoldButton(ButtonMappings button)
        {
            SetButtonState(button, true);
        }

        public void ReleaseButton(ButtonMappings button)
        {
            SetButtonState(button, false);
        }
    
        public void PressDPad(DPadMappings direction, int? delay = null)
        {
            HoldDPad(direction);
            Wait(delay);
            ReleaseDPad(direction);
        }

        public void HoldDPad(DPadMappings direction)
        {
            SetDPadDirection(direction, true);
        }

        public void ReleaseDPad(DPadMappings direction)
        {
            SetDPadDirection(direction, false);
        }
    
        public void PressStick(AxisMappings axis, byte value, int? delay = null)
        {
            HoldStick(axis, value);
            Wait(delay);
            ReleaseStick(axis);
        }
    
        public void HoldStick(AxisMappings axis, byte value)
        {
            SetAxisValue(axis, value);
        }

        public void ReleaseStick(AxisMappings axis)
        {
            SetAxisValue(axis, 128);
        }
        
        public void HoldTrigger(TriggerMappings trigger, byte value)
        {
            SetTriggerValue(trigger, value);
        }

        public void ReleaseTrigger(TriggerMappings trigger)
        {
            SetTriggerValue(trigger, 128);
        }
    
        private void SetDPadDirection(DPadMappings direction, bool pressed)
        {
            controller.SetDPadState(direction, pressed);
            Wait(delayAfterSet);
        }

        private void SetButtonState(ButtonMappings button, bool pressed)
        {
            controller.SetButtonState(button, pressed);
            Wait(delayAfterSet);
        }

        private void SetAxisValue(AxisMappings axis, byte value)
        {
            controller.SetAxisState(axis, value);
            Wait(delayAfterSet);
        }

        private void SetTriggerValue(TriggerMappings trigger, byte value)
        {
            controller.SetTriggerState(trigger, value);
            Wait(delayAfterSet);
        }

        private void Wait(int? delay)
        {
            stopwatch.Wait(delay ?? pressLength);
        }
    }
}