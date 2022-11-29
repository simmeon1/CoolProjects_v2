﻿using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Mappings;

namespace Vigem_Controllers
{
    public class Dualshock4Controller : IController
    {
        private readonly IDualShock4Controller controller;
        private DualShock4DPadDirection dpadState = DualShock4DPadDirection.None;
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

        public void SetDPadState(DPadMappings direction, bool pressed)
        {
            DualShock4DPadDirection newDpadState = GetDs4StateFromDpadAction(direction, pressed);
            controller.SetDPadDirection(newDpadState);
            dpadState = newDpadState;
        }

        private DualShock4DPadDirection GetDs4StateFromDpadAction(DPadMappings direction, bool pressed)
        {
            return GetDpadFromMapping(direction, pressed);
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

        private DualShock4DPadDirection GetDpadFromMapping(DPadMappings direction, bool pressed)
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