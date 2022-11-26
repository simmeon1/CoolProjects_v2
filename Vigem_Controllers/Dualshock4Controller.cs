using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Mappings;

namespace Vigem_Controllers
{
    public class Dualshock4Controller : IController
    {
        private readonly IDualShock4Controller controller;
        public Dualshock4Controller()
        {
            ViGEmClient client = new();
            controller = client.CreateDualShock4Controller();
        }

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


        public void SetDPadState(DPadStateMappings direction)
        {
            controller.SetDPadDirection(GetDpadFromMapping(direction));
        }

        private static DualShock4Axis GetAxisFromMapping(AxisMappings axis)
        {
            return axis switch
            {
                AxisMappings.LeftThumbX => DualShock4Axis.LeftThumbX,
                AxisMappings.LeftThumbY => DualShock4Axis.LeftThumbY,
                AxisMappings.RightThumbX => DualShock4Axis.RightThumbX,
                AxisMappings.RightThumbY => DualShock4Axis.RightThumbY,
                _ => throw new ArgumentException($"Axis mapping {axis} not supported."),
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
                ButtonMappings.Square => DualShock4Button.Square,
                _ => throw new ArgumentException($"Button mapping {button} not supported."),
            };
        }

        private static DualShock4DPadDirection GetDpadFromMapping(DPadStateMappings direction)
        {
            return direction switch
            {
                DPadStateMappings.None => DualShock4DPadDirection.None,
                DPadStateMappings.Northwest => DualShock4DPadDirection.Northwest,
                DPadStateMappings.West => DualShock4DPadDirection.West,
                DPadStateMappings.Southwest => DualShock4DPadDirection.Southwest,
                DPadStateMappings.South => DualShock4DPadDirection.South,
                DPadStateMappings.Southeast => DualShock4DPadDirection.Southeast,
                DPadStateMappings.East => DualShock4DPadDirection.East,
                DPadStateMappings.Northeast => DualShock4DPadDirection.Northeast,
                DPadStateMappings.North => DualShock4DPadDirection.North,
                _ => throw new ArgumentException($"Dpad mapping {direction} not supported."),
            };
        }
    }
}