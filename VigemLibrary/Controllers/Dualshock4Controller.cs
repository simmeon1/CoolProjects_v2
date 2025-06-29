using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using VigemLibrary.Mappings;

namespace VigemLibrary.Controllers
{
    public class Dualshock4Controller(IDualShock4Controller controller) : IController
    {
        private DualShock4DPadDirection dpadState = DualShock4DPadDirection.None;

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
            DualShock4DPadDirection newDpadState = GetDs4StateFromDpadAction(direction, pressed);
            controller.SetDPadDirection(newDpadState);
            dpadState = newDpadState;
        }

        public void SetTriggerState(TriggerMappings trigger, byte value)
        {
            //controller.SetSliderValue(GetTriggerFromMapping(trigger), value);
            controller.SetButtonState(GetTriggerFromMapping(trigger), value > 0);
        }

        //private static DualShock4Slider GetTriggerFromMapping(TriggerMappings trigger)
        //{
        //    return trigger switch
        //    {
        //        TriggerMappings.LeftTrigger => DualShock4Slider.LeftTrigger,
        //        _ => DualShock4Slider.RightTrigger
        //    };
        //}

        private static DualShock4Button GetTriggerFromMapping(TriggerMappings trigger)
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
                //ButtonMappings.TriggerRight => DualShock4Button.TriggerRight,
                //ButtonMappings.TriggerLeft => DualShock4Button.TriggerLeft,
                ButtonMappings.ShoulderRight => DualShock4Button.ShoulderRight,
                ButtonMappings.ShoulderLeft => DualShock4Button.ShoulderLeft,
                ButtonMappings.Triangle => DualShock4Button.Triangle,
                ButtonMappings.Circle => DualShock4Button.Circle,
                ButtonMappings.Cross => DualShock4Button.Cross,
                _ => DualShock4Button.Square
            };
        }

        private DualShock4DPadDirection GetDs4StateFromDpadAction(DPadMappings direction, bool pressed)
        {
            switch (direction)
            {
                case DPadMappings.Up:
                    return GetDs4DpadFromMapping(
                        DualShock4DPadDirection.West, DualShock4DPadDirection.Northwest,
                        DualShock4DPadDirection.East, DualShock4DPadDirection.Northeast,
                        DualShock4DPadDirection.North, pressed
                    );
                case DPadMappings.Down:
                    return GetDs4DpadFromMapping(
                        DualShock4DPadDirection.West, DualShock4DPadDirection.Southwest,
                        DualShock4DPadDirection.East, DualShock4DPadDirection.Southeast,
                        DualShock4DPadDirection.South, pressed
                    );
                case DPadMappings.Left:
                    return GetDs4DpadFromMapping(
                        DualShock4DPadDirection.North, DualShock4DPadDirection.Northwest,
                        DualShock4DPadDirection.South, DualShock4DPadDirection.Southwest,
                        DualShock4DPadDirection.West, pressed
                    );
                default:
                    return GetDs4DpadFromMapping(
                        DualShock4DPadDirection.North, DualShock4DPadDirection.Northeast,
                        DualShock4DPadDirection.South, DualShock4DPadDirection.Southeast,
                        DualShock4DPadDirection.East, pressed
                    );
            }
        }

        private DualShock4DPadDirection GetDs4DpadFromMapping(
            DualShock4DPadDirection ifPressed1,
            DualShock4DPadDirection thenPressed1,
            DualShock4DPadDirection ifPressed2,
            DualShock4DPadDirection thenPressed2,
            DualShock4DPadDirection elseIfPressed,
            bool pressed
        )
        {
            if (pressed)
            {
                if (dpadState == ifPressed1) return thenPressed1;
                if (dpadState == ifPressed2) return thenPressed2;
                return elseIfPressed;
            }
            else
            {
                if (dpadState == thenPressed1) return ifPressed1;
                if (dpadState == thenPressed2) return ifPressed2;
                return DualShock4DPadDirection.None;
            }
        }
    }
}