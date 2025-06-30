using SharpDX.DirectInput;
using VigemLibrary.Mappings;

namespace VigemLibrary
{
    public class CustomControllerUser(StopwatchControllerUser controllerUser)
    {
        private readonly Dictionary<int, ButtonMappings> buttonMappings = new()
        {
            {0, ButtonMappings.Square},
            {1, ButtonMappings.Cross},
            {2, ButtonMappings.Circle},
            {3, ButtonMappings.Triangle},
            {4, ButtonMappings.ShoulderLeft},
            {5, ButtonMappings.ShoulderRight},
            // Not set
            // { 6, ButtonMappings.LeftTrigger },
            // { 7, ButtonMappings.RightTrigger },
            {8, ButtonMappings.Share},
            {9, ButtonMappings.Options},
            {10, ButtonMappings.ThumbLeft},
            {11, ButtonMappings.ThumbRight},
            // {12, ButtonMappings.Home },
            // {13, ButtonMappings.Map },
        };

        public void Create()
        {
            // Based on https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectInput/JoystickApp/Program.cs if you need joystick
            // Removed joystick and buffered code.
            
            // Initialize DirectInput
            var directInput = new DirectInput();

            var joystickGuid = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices).Single().InstanceGuid;
            // If Gamepad not found, look for a Joystick

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);
            
            // Query all supported ForceFeedback effects
            // Zero for dualsense so code removed

            // Set BufferSize in order to use buffered data.
            // UPDATE WHEN READING BUFFERED DATA
            // joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();
                var currentState = joystick.GetCurrentState();
                HandleButtons(currentState);
                HandleDpad(currentState);
                HandleAxis(currentState);
                HandleTriggers(currentState);
            }
        }

        private void HandleTriggers(JoystickState s)
        {
            void HoldStick(TriggerMappings triggerMapping, int value)
            {
                controllerUser.HoldTrigger(triggerMapping, (byte) Math.Floor(value / 257.0));
            }

            HoldStick(TriggerMappings.LeftTrigger, s.RotationX);
            HoldStick(TriggerMappings.RightTrigger, s.RotationY);
        }

        private void HandleAxis(JoystickState s)
        {
            void HoldStick(AxisMappings axisMappings, int value)
            {
                controllerUser.HoldStick(axisMappings, (byte) Math.Floor(value / 257.0));
            }

            HoldStick(AxisMappings.LeftThumbX, s.X);
            HoldStick(AxisMappings.LeftThumbY, s.Y);
            HoldStick(AxisMappings.RightThumbX, s.Z);
            HoldStick(AxisMappings.RightThumbY, s.RotationZ);
        }

        private void HandleDpad(JoystickState s)
        {
            var state = s.PointOfViewControllers[0];
            if (state == -1)
            {
                controllerUser.ReleaseDPad(DPadMappings.Up);
                controllerUser.ReleaseDPad(DPadMappings.Right);
                controllerUser.ReleaseDPad(DPadMappings.Down);
                controllerUser.ReleaseDPad(DPadMappings.Left);
                return;
            }

            void ToggleDpadDirection(int state1, int state2, int state3, DPadMappings dPadMappings)
            {
                if (state == state1 || state == state2 || state == state3)
                {
                    controllerUser.HoldDPad(dPadMappings);
                }
                else
                {
                    controllerUser.ReleaseDPad(dPadMappings);
                }
            }

            ToggleDpadDirection(31500, 0, 4500, DPadMappings.Up);
            ToggleDpadDirection(4500, 9000, 13500, DPadMappings.Right);
            ToggleDpadDirection(13500, 18000, 22500, DPadMappings.Down);
            ToggleDpadDirection(22500, 27000, 31500, DPadMappings.Left);
        }

        private void HandleButtons(JoystickState s)
        {
            var shoulderRight = ButtonMappings.ShoulderRight;
            var shoulderRightIndex = buttonMappings.Single(x => x.Value == shoulderRight).Key;
            var shoulderRightButtonIsPressed = s.Buttons[shoulderRightIndex];
            foreach (var mapping in buttonMappings)
            {
                var buttonIndex = mapping.Key;
                var button = mapping.Value;
                var buttonIsPressed = s.Buttons[buttonIndex];
                if (button == shoulderRight || !shoulderRightButtonIsPressed)
                {
                    if (buttonIsPressed)
                    {
                        controllerUser.HoldButton(button);
                    }
                    else
                    {
                        controllerUser.ReleaseButton(button);
                    }
                } else if (buttonIsPressed && shoulderRightButtonIsPressed)
                {
                    controllerUser.PressButton(button);
                    controllerUser.Wait(50);
                }
            }
        }
    }
}