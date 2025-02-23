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

        public StopwatchControllerUser(IController controller, IStopwatch stopwatch, int pressLength)
        {
            this.controller = controller;
            this.stopwatch = stopwatch;
            this.pressLength = pressLength;
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

        public void PressButton(ButtonMappings button, int? delay = null)
        {
            HoldButton(button);
            Wait(delay);
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
    
        private void SetDPadDirection(DPadMappings direction, bool pressed)
        {
            controller.SetDPadState(direction, pressed);
        }

        private void SetButtonState(ButtonMappings button, bool pressed)
        {
            controller.SetButtonState(button, pressed);
        }

        private void SetAxisValue(AxisMappings axis, byte value)
        {
            controller.SetAxisState(axis, value);
        }

        private void Wait(int? delay)
        {
            stopwatch.Wait(delay ?? pressLength);
        }
    }
}