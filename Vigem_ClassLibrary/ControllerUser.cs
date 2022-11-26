using Vigem_ClassLibrary.Mappings;

namespace Vigem_ClassLibrary
{
    public class ControllerUser
    {
        private readonly IController controller;
        private readonly IDelayer delayer;
        private readonly int pressLength;

        public ControllerUser(IController controller, IDelayer delayer, int pressLength)
        {
            this.controller = controller;
            this.delayer = delayer;
            this.pressLength = pressLength;
        }

        public void Connect()
        {
            controller.Connect();
        }
        
        public void Disconnect()
        {
            controller.Disconnect();
        }

        public async Task PressButton(ButtonMappings button, int? delay = null)
        {
            HoldButton(button);
            await Wait(delay);
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
    
        public async Task PressDPad(DPadStateMappings direction, int? delay = null)
        {
            HoldDPad(direction);
            await Wait(delay);
            ReleaseDPad();
        }

        public void HoldDPad(DPadStateMappings direction)
        {
            SetDPadDirection(direction);
        }

        public void ReleaseDPad()
        {
            SetDPadDirection(DPadStateMappings.None);
        }
    
        public async Task PressStick(AxisMappings axis, byte value, int? delay = null)
        {
            HoldStick(axis, value);
            await Wait(delay);
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
    
        private void SetDPadDirection(DPadStateMappings direction)
        {
            controller.SetDPadState(direction);
        }

        private void SetButtonState(ButtonMappings button, bool pressed)
        {
            controller.SetButtonState(button, pressed);
        }

        private void SetAxisValue(AxisMappings axis, byte value)
        {
            controller.SetAxisState(axis, value);
        }

        private Task Wait(int? delay)
        {
            return delayer.Delay(delay ?? pressLength);
        }
    }
}