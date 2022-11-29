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

        public void SetDPadState(DPadMappings mapping, bool pressed)
        {
            controller.SetButtonState(GetDpadFromMapping(mapping), pressed);
        }

        private static Xbox360Button GetDpadFromMapping(DPadMappings mapping)
        {

            switch (mapping)
            {
                case DPadMappings.Up:
                    return Xbox360Button.Up;
                case DPadMappings.Down:
                    return Xbox360Button.Down;
                case DPadMappings.Left:
                    return Xbox360Button.Left;
                case DPadMappings.Right:
                    return Xbox360Button.Right;
                default:
                    throw new ArgumentException($"Dpad mapping {mapping} not supported.");
            }
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