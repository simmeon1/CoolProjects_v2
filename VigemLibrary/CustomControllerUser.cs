using SharpDX.DirectInput;
using VigemLibrary.Mappings;

namespace VigemLibrary
{
    public class CustomControllerUser(StopwatchControllerUser controllerUser)
    {
        private readonly StopwatchControllerUser controllerUser = controllerUser;

        public void Listen()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            var joystickGuid = directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices).Single().InstanceGuid;
            // If Gamepad not found, look for a Joystick
            // Lookup https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectInput/JoystickApp/Program.cs if you need joystick

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);
            
            // Query all suported ForceFeedback effects
            // Zero for dualsense so code removed

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;
            // joystick.Properties.BufferSize = 500;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                foreach (var state in datas)
                {
                    HandleDpad(state);
                }
                // Console.WriteLine(DateTime.Now + " - " + datas.Length + " - " + state.Value);
            }
        }

        private void HandleDpad(JoystickUpdate state)
        {
            if (state.Offset is not JoystickOffset.PointOfViewControllers0)
            {
                return;
            }

            void ReleaseAllDpadExcept(params DPadMappings[] mappingsToExclude)
            {
                var allMappings = new [] { DPadMappings.Up, DPadMappings.Right, DPadMappings.Down, DPadMappings.Left };
                foreach (var mapping in allMappings)
                {
                    if (!mappingsToExclude.Contains(mapping))
                    {
                        controllerUser.ReleaseDPad(mapping);
                    }
                }
            }

            void PressDpads(params DPadMappings[] mappingsToInclude)
            {
                ReleaseAllDpadExcept(mappingsToInclude);
                foreach (var mapping in mappingsToInclude)
                {
                    if (mappingsToInclude.Contains(mapping))
                    {
                        controllerUser.HoldDPad(mapping);
                    }
                }
            }

            if (state.Value == -1) ReleaseAllDpadExcept();
            else if (state.Value == 0) PressDpads(DPadMappings.Up); 
            else if (state.Value == 4500) PressDpads(DPadMappings.Up, DPadMappings.Right);
            else if (state.Value == 9000) PressDpads(DPadMappings.Right); 
            else if (state.Value == 13500) PressDpads(DPadMappings.Right, DPadMappings.Down);
            else if (state.Value == 18000) PressDpads(DPadMappings.Down); 
            else if (state.Value == 22500) PressDpads(DPadMappings.Down, DPadMappings.Left);
            else if (state.Value == 27000) PressDpads(DPadMappings.Left); 
            else if (state.Value == 31500) PressDpads(DPadMappings.Left, DPadMappings.Up);
        }
    }
}