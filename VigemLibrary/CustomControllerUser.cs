using System.Text.Json;
using SharpDX.DirectInput;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;
using VigemLibrary.SystemImplementations;

namespace VigemLibrary;

public class CustomControllerUser {
    private readonly Dictionary<int, ButtonMappings> buttonMappings = new() {
        {0, ButtonMappings.Square},
        {1, ButtonMappings.Cross},
        {2, ButtonMappings.Circle},
        {3, ButtonMappings.Triangle},
        {4, ButtonMappings.ShoulderLeft},
        {5, ButtonMappings.ShoulderRight},
        // Triggers covered by slider functionality
        // { 6, ButtonMappings.LeftTrigger },
        // { 7, ButtonMappings.RightTrigger },
        {8, ButtonMappings.Share},
        {9, ButtonMappings.Options},
        {10, ButtonMappings.ThumbLeft},
        {11, ButtonMappings.ThumbRight},
        {12, ButtonMappings.Ps},
        {13, ButtonMappings.Touchpad}
    };

    private readonly StopwatchControllerUser controllerUser;
    private readonly IStopwatch s;

    private readonly Dictionary<ButtonMappings, TimestampedTurbo> turboedButtons = new();
    private bool turboPressed;

    public CustomControllerUser(IStopwatch s, StopwatchControllerUser controllerUser) {
        this.s = s;
        this.controllerUser = controllerUser;
    }

    public void Create() {
        // Based on https://github.com/sharpdx/SharpDX-Samples/blob/master/Desktop/DirectInput/JoystickApp/Program.cs if you need joystick
        // Removed joystick and buffered code.
        // Commented out poll method, doesnt look needed.

        // Initialize DirectInput
        var directInput = new DirectInput();

        DeviceInstance? GetDevice() => directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)
            .SingleOrDefault();

        var device = GetDevice();
        while (device == null) {
            Console.WriteLine("Controller not found. Is it hidden by HidHide? Retrying in 5 seconds.");
            s.Wait(5000);
            device = GetDevice();
        }

        var joystickGuid = device.InstanceGuid;
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
        var lastEmulatedControllerState = controllerUser.GetState();
        while (true) {
            // joystick.Poll();
            var currentState = joystick.GetCurrentState();
            HandleButtons(currentState);
            HandleDpad(currentState);
            HandleAxis(currentState);
            HandleTriggers(currentState);
            lastEmulatedControllerState = HandleStateUpdate(lastEmulatedControllerState);
        }
    }

    private ControllerState HandleStateUpdate(ControllerState lastEmulatedControllerState) {
        var emulatedControllerState = controllerUser.GetState();
        var emulatedUpdate = new ControllerState(
            GetDiff(lastEmulatedControllerState.axisStates, emulatedControllerState.axisStates),
            GetDiff(lastEmulatedControllerState.buttonStates, emulatedControllerState.buttonStates),
            GetDiff(lastEmulatedControllerState.dpadStates, emulatedControllerState.dpadStates),
            GetDiff(lastEmulatedControllerState.triggerStates, emulatedControllerState.triggerStates)
        );
        if (emulatedUpdate.axisStates.Count != 0 || emulatedUpdate.buttonStates.Count != 0 ||
            emulatedUpdate.dpadStates.Count != 0 || emulatedUpdate.triggerStates.Count != 0) {
            Console.WriteLine(JsonSerializer.Serialize(emulatedUpdate));
        }
        return emulatedControllerState;
    }

    private void HandleTriggers(JoystickState s) {
        void SetTriggerValue(TriggerMappings triggerMapping, int value) {
            var floored = (byte) Math.Floor(value / 257.0);
            controllerUser.SetTrigger(
                triggerMapping,
                floored > 200 ? byte.MaxValue : byte.MinValue
            );
        }

        SetTriggerValue(TriggerMappings.LeftTrigger, s.RotationX);
        SetTriggerValue(TriggerMappings.RightTrigger, s.RotationY);
    }

    private void HandleAxis(JoystickState s) {
        void SetAxisValue(AxisMappings axisMappings, int value) {
            var floored = (byte) Math.Floor(value / 257.0);
            controllerUser.SetAxis(
                axisMappings,
                floored > 200 ? byte.MaxValue : floored < 50 ? byte.MinValue : (byte) 128
            );
        }

        SetAxisValue(AxisMappings.LeftThumbX, s.X);
        SetAxisValue(AxisMappings.LeftThumbY, s.Y);
        SetAxisValue(AxisMappings.RightThumbX, s.Z);
        SetAxisValue(AxisMappings.RightThumbY, s.RotationZ);
    }

    private void HandleDpad(JoystickState s) {
        var state = s.PointOfViewControllers[0];
        if (state == -1) {
            controllerUser.ReleaseDPad(DPadMappings.Up);
            controllerUser.ReleaseDPad(DPadMappings.Right);
            controllerUser.ReleaseDPad(DPadMappings.Down);
            controllerUser.ReleaseDPad(DPadMappings.Left);
            return;
        }

        void ToggleDpadDirection(int state1, int state2, int state3, DPadMappings dPadMappings) {
            controllerUser.SetDPadDirection(dPadMappings, state == state1 || state == state2 || state == state3);
        }

        ToggleDpadDirection(31500, 0, 4500, DPadMappings.Up);
        ToggleDpadDirection(4500, 9000, 13500, DPadMappings.Right);
        ToggleDpadDirection(13500, 18000, 22500, DPadMappings.Down);
        ToggleDpadDirection(22500, 27000, 31500, DPadMappings.Left);
    }

    private void HandleButtons(JoystickState s) {
        foreach (var mapping in buttonMappings) {
            var buttonIndex = mapping.Key;
            var button = mapping.Value;
            var buttonIsPressed = s.Buttons[buttonIndex];

            var buttonIsTurbo = button == ButtonMappings.ShoulderRight;
            if (buttonIsTurbo) {
                turboPressed = buttonIsPressed;
            }

            if (buttonIsTurbo || !turboPressed || !buttonIsPressed) {
                controllerUser.SetButtonState(button, buttonIsPressed);
                continue;
            }

            if (!turboedButtons.ContainsKey(button)) {
                turboedButtons.Add(button, new TimestampedTurbo(this.s.GetElapsedTotalMilliseconds(), buttonIsPressed));
            }
            var turboedButton = turboedButtons[button];
            if (this.s.GetElapsedTotalMilliseconds() - turboedButton.Timestamp > 50) {
                turboedButtons[button] = new TimestampedTurbo(
                    this.s.GetElapsedTotalMilliseconds(),
                    !turboedButtons[button].Pressed
                );
                var buttonIsPressed1 = turboedButtons[button].Pressed;
                controllerUser.SetButtonState(button, buttonIsPressed1);
            }
        }
    }

    private static Dictionary<T, TV> GetDiff<T, TV>(IReadOnlyDictionary<T, TV> d1, IReadOnlyDictionary<T, TV> d2)
        where T : notnull where TV : notnull =>
        d1.Aggregate(
            new Dictionary<T, TV>(),
            (acc, v) => {
                if (!v.Value.Equals(d2[v.Key])) {
                    acc.Add(v.Key, d2[v.Key]);
                }
                return acc;
            }
        );

    private class TimestampedTurbo(double timestamp, bool pressed) {
        public double Timestamp { get; } = timestamp;
        public bool Pressed { get; } = pressed;
    }
}