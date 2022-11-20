using System;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace Vigem_ClassLibrary
{
    public class Ds4ControllerUser
    {
        private readonly IDualShock4ControllerWrapper controller;
        private int pressLength;

        public Ds4ControllerUser(IDualShock4ControllerWrapper controllerWrapper, int pressLength = 200)
        {
            controller = controllerWrapper;
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
            delay ??= pressLength;
            HoldButton(button);
            await Task.Delay(delay.Value);
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
    
        public async Task PressDPad(DPadMappings direction, int? delay = null)
        {
            delay ??= pressLength;
            HoldDPad(direction);
            await Task.Delay(delay.Value);
            ReleaseDPad();
        }

        public void HoldDPad(DPadMappings direction)
        {
            SetDPadDirection(direction);
        }

        public void ReleaseDPad()
        {
            SetDPadDirection(DPadMappings.None);
        }
    
        public async Task PressStick(AxisMappings axis, byte value, int? delay = null)
        {
            delay ??= pressLength;
            HoldStick(axis, value);
            await Task.Delay(delay.Value);
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
    
        private void SetDPadDirection(DPadMappings direction)
        {
            controller.SetDPadDirection(GetDpadFromMapping(direction));
        }


        private void SetButtonState(ButtonMappings button, bool pressed)
        {
            controller.SetButtonState(GetButtonFromMapping(button), pressed);
        }
    
        private void SetAxisValue(AxisMappings axis, byte value)
        {
            controller.SetAxisValue(GetAxisFromMapping(axis), value);
        }

        private DualShock4Button GetButtonFromMapping(ButtonMappings mapping)
        {
            switch (mapping)
            {
                case ButtonMappings.ThumbRight:
                    return DualShock4Button.ThumbRight;
                case ButtonMappings.ThumbLeft:
                    return DualShock4Button.ThumbLeft;
                case ButtonMappings.Options:
                    return DualShock4Button.Options;
                case ButtonMappings.Share:
                    return DualShock4Button.Share;
                case ButtonMappings.TriggerRight:
                    return DualShock4Button.TriggerRight;
                case ButtonMappings.TriggerLeft:
                    return DualShock4Button.TriggerLeft;
                case ButtonMappings.ShoulderRight:
                    return DualShock4Button.ShoulderRight;
                case ButtonMappings.ShoulderLeft:
                    return DualShock4Button.ShoulderLeft;
                case ButtonMappings.Triangle:
                    return DualShock4Button.Triangle;
                case ButtonMappings.Circle:
                    return DualShock4Button.Circle;
                case ButtonMappings.Cross:
                    return DualShock4Button.Cross;
                case ButtonMappings.Square:
                    return DualShock4Button.Square;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapping), mapping, null);
            }
        
        }
        private DualShock4DPadDirection GetDpadFromMapping(DPadMappings mapping)
        {
            switch (mapping)
            {
                case DPadMappings.None:
                    return DualShock4DPadDirection.None;
                case DPadMappings.Northwest:
                    return DualShock4DPadDirection.Northwest;
                case DPadMappings.West:
                    return DualShock4DPadDirection.West;
                case DPadMappings.Southwest:
                    return DualShock4DPadDirection.Southwest;
                case DPadMappings.South:
                    return DualShock4DPadDirection.South;
                case DPadMappings.Southeast:
                    return DualShock4DPadDirection.Southeast;
                case DPadMappings.East:
                    return DualShock4DPadDirection.East;
                case DPadMappings.Northeast:
                    return DualShock4DPadDirection.Northeast;
                case DPadMappings.North:
                    return DualShock4DPadDirection.North;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapping), mapping, null);
            }
        }

        private DualShock4Axis GetAxisFromMapping(AxisMappings mapping)
        {
            switch (mapping)
            {
                case AxisMappings.LeftThumbX:
                    return DualShock4Axis.LeftThumbX;
                case AxisMappings.LeftThumbY:
                    return DualShock4Axis.LeftThumbY;
                case AxisMappings.RightThumbX:
                    return DualShock4Axis.RightThumbX;
                case AxisMappings.RightThumbY:
                    return DualShock4Axis.RightThumbY;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapping), mapping, null);
            }
        }
    }
}