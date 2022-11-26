using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Mappings;

namespace Vigem_Controllers
{
    public class Xbox360Controller : IController
    {
        private readonly IXbox360Controller controller;
        public Xbox360Controller()
        {
            ViGEmClient client = new();
            controller = client.CreateXbox360Controller();
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
            controller.SetAxisValue(GetAxisFromMapping(axis), Convert.ToInt16((value - 128) * 256));
        }

        public void SetButtonState(ButtonMappings button, bool pressed)
        {
            controller.SetButtonState(GetButtonFromMapping(button), pressed);
        }


        public void SetDPadState(DPadMappings direction)
        {
            Xbox360Button up = Xbox360Button.Up;
            Xbox360Button down = Xbox360Button.Down;
            Xbox360Button left = Xbox360Button.Left;
            Xbox360Button right = Xbox360Button.Right;

            foreach (Xbox360Button dpadButton in new[] { up, down, left, right })
            {
                controller.SetButtonState(dpadButton, false);
            }

            if (direction == DPadMappings.Northwest) HoldButtons(up, left);
            else if (direction == DPadMappings.West) HoldButtons(left);
            else if (direction == DPadMappings.Southwest) HoldButtons(down, left);
            else if (direction == DPadMappings.South) HoldButtons(down);
            else if (direction == DPadMappings.Southeast) HoldButtons(down, right);
            else if (direction == DPadMappings.East) HoldButtons(right);
            else if (direction == DPadMappings.Northeast) HoldButtons(up, right);
            else if (direction == DPadMappings.North) HoldButtons(up);
            else if (direction == DPadMappings.None) { }
            else throw new ArgumentException($"Dpad mapping {direction} not supported.");
        }

        private void HoldButtons(params Xbox360Button[] buttons)
        {
            foreach (Xbox360Button button in buttons) controller.SetButtonState(button, true);
        }

        private static Xbox360Axis GetAxisFromMapping(AxisMappings axis)
        {
            return axis switch
            {
                AxisMappings.LeftThumbX => Xbox360Axis.LeftThumbX,
                AxisMappings.LeftThumbY => Xbox360Axis.LeftThumbY,
                AxisMappings.RightThumbX => Xbox360Axis.RightThumbX,
                AxisMappings.RightThumbY => Xbox360Axis.RightThumbY,
                _ => throw new ArgumentException($"Axis mapping {axis} not supported."),
            };
        }

        private static Xbox360Button GetButtonFromMapping(ButtonMappings button)
        {
            return button switch
            {
                ButtonMappings.ThumbRight => Xbox360Button.RightThumb,
                ButtonMappings.ThumbLeft => Xbox360Button.LeftThumb,
                ButtonMappings.Options => Xbox360Button.Start,
                ButtonMappings.Share => Xbox360Button.Back,
                //ButtonMappings.TriggerRight => Xbox360Button.TriggerRight,
                //ButtonMappings.TriggerLeft => Xbox360Button.TriggerLeft,
                ButtonMappings.ShoulderRight => Xbox360Button.RightShoulder,
                ButtonMappings.ShoulderLeft => Xbox360Button.LeftShoulder,
                ButtonMappings.Triangle => Xbox360Button.Y,
                ButtonMappings.Circle => Xbox360Button.B,
                ButtonMappings.Cross => Xbox360Button.A,
                ButtonMappings.Square => Xbox360Button.X,
                _ => throw new ArgumentException($"Button mapping {button} not supported."),
            };
        }
    }
}