using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using VigemLibrary.Mappings;

namespace VigemLibrary.Controllers
{
    public class Xbox360Controller : IController
    {
        private readonly IXbox360Controller controller;
        public Xbox360Controller(IXbox360Controller controller)
        {
            this.controller = controller;
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
            int x = value - 128;
            short result = 0;
            if (x != 0)
            {
                bool byteIsPositive = x >= 1;
                result = byteIsPositive ? Convert.ToInt16(short.MaxValue / 127.0 * x) : Convert.ToInt16(-(short.MinValue / 128.0 * x));
                if (axis is AxisMappings.LeftThumbY or AxisMappings.RightThumbY)
                {
                    double multiplier = byteIsPositive ? (double)short.MinValue / short.MaxValue : (double)short.MaxValue / short.MinValue;
                    result = Convert.ToInt16(result * multiplier);
                }
            }
            controller.SetAxisValue(GetAxisFromMapping(axis), result);
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

            return mapping switch
            {
                DPadMappings.Up => Xbox360Button.Up,
                DPadMappings.Down => Xbox360Button.Down,
                DPadMappings.Left => Xbox360Button.Left,
                _ => Xbox360Button.Right,
            };
        }

        private static Xbox360Axis GetAxisFromMapping(AxisMappings axis)
        {
            return axis switch
            {
                AxisMappings.LeftThumbX => Xbox360Axis.LeftThumbX,
                AxisMappings.LeftThumbY => Xbox360Axis.LeftThumbY,
                AxisMappings.RightThumbX => Xbox360Axis.RightThumbX,
                _ => Xbox360Axis.RightThumbY
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
                _ => Xbox360Button.X
            };
        }

        public void SetTriggerState(TriggerMappings trigger, byte value)
        {
            throw new NotImplementedException();
        }
    }
}