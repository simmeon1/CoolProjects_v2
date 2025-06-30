using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using VigemLibrary.Mappings;

namespace VigemLibrary.Controllers
{
    public class Dualshock4Controller(IDualShock4Controller controller) : IController
    {
        private bool dpadUpPressed;
        private bool dpadRightPressed;
        private bool dpadDownPressed;
        private bool dpadLeftPressed;

        public void Connect()
        {
            controller.Connect();
        }

        public void Disconnect()
        {
            controller.Disconnect();
        }

        public void SetAxisState(AxisMappings axis, byte value)
        {
            controller.SetAxisValue(GetAxisFromMapping(axis), value);
        }

        public void SetButtonState(ButtonMappings button, bool pressed)
        {
            controller.SetButtonState(GetButtonFromMapping(button), pressed);
        }

        public void SetDPadState(DPadMappings direction, bool pressed)
        {
            if (direction == DPadMappings.Up) dpadUpPressed = pressed;
            else if (direction == DPadMappings.Right) dpadRightPressed = pressed;
            else if (direction == DPadMappings.Down) dpadDownPressed = pressed;
            else if (direction == DPadMappings.Left) dpadLeftPressed = pressed;
            
            if (dpadUpPressed && dpadRightPressed) controller.SetDPadDirection(DualShock4DPadDirection.Northeast);
            else if (dpadRightPressed && dpadDownPressed) controller.SetDPadDirection(DualShock4DPadDirection.Southeast);
            else if (dpadDownPressed && dpadLeftPressed) controller.SetDPadDirection(DualShock4DPadDirection.Southwest);
            else if (dpadLeftPressed && dpadUpPressed) controller.SetDPadDirection(DualShock4DPadDirection.Northwest);
            else if (dpadUpPressed) controller.SetDPadDirection(DualShock4DPadDirection.North);
            else if (dpadRightPressed) controller.SetDPadDirection(DualShock4DPadDirection.East);
            else if (dpadDownPressed) controller.SetDPadDirection(DualShock4DPadDirection.South);
            else if (dpadLeftPressed) controller.SetDPadDirection(DualShock4DPadDirection.West);
            else controller.SetDPadDirection(DualShock4DPadDirection.None);
        }

        public void SetTriggerState(TriggerMappings trigger, byte value)
        {
            controller.SetButtonState(GetButtonFromSliderMapping(trigger), value > 0);
            controller.SetSliderValue(GetSliderFromMapping(trigger), value);
        }

        private static DualShock4Slider GetSliderFromMapping(TriggerMappings trigger)
        {
            return trigger switch
            {
                TriggerMappings.LeftTrigger => DualShock4Slider.LeftTrigger,
                _ => DualShock4Slider.RightTrigger
            };
        }
        
        private static DualShock4Button GetButtonFromSliderMapping(TriggerMappings trigger)
        {
            return trigger switch
            {
                TriggerMappings.LeftTrigger => DualShock4Button.TriggerLeft,
                _ => DualShock4Button.TriggerRight
            };
        }

        private static DualShock4Axis GetAxisFromMapping(AxisMappings axis)
        {
            return axis switch
            {
                AxisMappings.LeftThumbX => DualShock4Axis.LeftThumbX,
                AxisMappings.LeftThumbY => DualShock4Axis.LeftThumbY,
                AxisMappings.RightThumbX => DualShock4Axis.RightThumbX,
                _ => DualShock4Axis.RightThumbY
            };
        }

        private static DualShock4Button GetButtonFromMapping(ButtonMappings button)
        {
            return button switch
            {
                ButtonMappings.ThumbRight => DualShock4Button.ThumbRight,
                ButtonMappings.ThumbLeft => DualShock4Button.ThumbLeft,
                ButtonMappings.Options => DualShock4Button.Options,
                ButtonMappings.Share => DualShock4Button.Share,
                // ButtonMappings.TriggerRight => DualShock4Button.TriggerRight,
                // ButtonMappings.TriggerLeft => DualShock4Button.TriggerLeft,
                ButtonMappings.ShoulderRight => DualShock4Button.ShoulderRight,
                ButtonMappings.ShoulderLeft => DualShock4Button.ShoulderLeft,
                ButtonMappings.Triangle => DualShock4Button.Triangle,
                ButtonMappings.Circle => DualShock4Button.Circle,
                ButtonMappings.Cross => DualShock4Button.Cross,
                ButtonMappings.Ps => DualShock4SpecialButton.Ps,
                ButtonMappings.Touchpad => DualShock4SpecialButton.Touchpad,
                _ => DualShock4Button.Square
            };
        }
    }
}