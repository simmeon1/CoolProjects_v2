using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using VigemLibrary.Mappings;

namespace VigemLibrary.Controllers;

public class Dualshock4Controller(IDualShock4Controller controller) : IController {
    private readonly Dictionary<AxisMappings, byte> axisStates = InitByteStates<AxisMappings>(128);
    private readonly Dictionary<ButtonMappings, bool> buttonStates = InitBoolStates<ButtonMappings>();
    private readonly Dictionary<DPadMappings, bool> dpadStates = InitBoolStates<DPadMappings>();
    private readonly Dictionary<TriggerMappings, byte> triggerStates = InitByteStates<TriggerMappings>(0);
    private bool dpadDownPressed;
    private bool dpadLeftPressed;
    private bool dpadRightPressed;
    private bool dpadUpPressed;

    public void Connect() {
        controller.Connect();
    }

    public void Disconnect() {
        controller.Disconnect();
    }

    public void SetAxisState(AxisMappings axis, byte value) {
        controller.SetAxisValue(GetAxisFromMapping(axis), value);
        axisStates[axis] = value;
    }

    public void SetButtonState(ButtonMappings button, bool pressed) {
        controller.SetButtonState(GetButtonFromMapping(button), pressed);
        buttonStates[button] = pressed;
    }

    public void SetDPadState(DPadMappings direction, bool pressed) {
        if (direction == DPadMappings.Up) {
            dpadUpPressed = pressed;
        }
        else if (direction == DPadMappings.Right) {
            dpadRightPressed = pressed;
        }
        else if (direction == DPadMappings.Down) {
            dpadDownPressed = pressed;
        }
        else if (direction == DPadMappings.Left) {
            dpadLeftPressed = pressed;
        }

        controller.SetDPadDirection(
            dpadUpPressed && dpadRightPressed ? DualShock4DPadDirection.Northeast :
            dpadRightPressed && dpadDownPressed ? DualShock4DPadDirection.Southeast :
            dpadDownPressed && dpadLeftPressed ? DualShock4DPadDirection.Southwest :
            dpadLeftPressed && dpadUpPressed ? DualShock4DPadDirection.Northwest :
            dpadUpPressed ? DualShock4DPadDirection.North :
            dpadRightPressed ? DualShock4DPadDirection.East :
            dpadDownPressed ? DualShock4DPadDirection.South :
            dpadLeftPressed ? DualShock4DPadDirection.West :
            DualShock4DPadDirection.None
        );
        dpadStates[direction] = pressed;
    }

    public void SetTriggerState(TriggerMappings trigger, byte value) {
        controller.SetButtonState(GetButtonFromSliderMapping(trigger), value > 0);
        controller.SetSliderValue(GetSliderFromMapping(trigger), value);
        triggerStates[trigger] = value;
    }

    public ControllerState GetState() => new(
        new Dictionary<AxisMappings, byte>(axisStates),
        new Dictionary<ButtonMappings, bool>(buttonStates),
        new Dictionary<DPadMappings, bool>(dpadStates),
        new Dictionary<TriggerMappings, byte>(triggerStates)
    );

    private static Dictionary<T, byte> InitByteStates<T>(byte defaultByte) where T : struct, Enum =>
        new(Enum.GetValues<T>().Select(m => new KeyValuePair<T, byte>(m, defaultByte)));

    private static Dictionary<T, bool> InitBoolStates<T>() where T : struct, Enum =>
        new(Enum.GetValues<T>().Select(m => new KeyValuePair<T, bool>(m, false)));

    private static DualShock4Slider GetSliderFromMapping(TriggerMappings trigger) =>
        trigger switch {
            TriggerMappings.LeftTrigger => DualShock4Slider.LeftTrigger,
            _ => DualShock4Slider.RightTrigger
        };

    private static DualShock4Button GetButtonFromSliderMapping(TriggerMappings trigger) =>
        trigger switch {
            TriggerMappings.LeftTrigger => DualShock4Button.TriggerLeft,
            _ => DualShock4Button.TriggerRight
        };

    private static DualShock4Axis GetAxisFromMapping(AxisMappings axis) =>
        axis switch {
            AxisMappings.LeftThumbX => DualShock4Axis.LeftThumbX,
            AxisMappings.LeftThumbY => DualShock4Axis.LeftThumbY,
            AxisMappings.RightThumbX => DualShock4Axis.RightThumbX,
            _ => DualShock4Axis.RightThumbY
        };

    private static DualShock4Button GetButtonFromMapping(ButtonMappings button) =>
        button switch {
            ButtonMappings.ThumbRight => DualShock4Button.ThumbRight,
            ButtonMappings.ThumbLeft => DualShock4Button.ThumbLeft,
            ButtonMappings.Options => DualShock4Button.Options,
            ButtonMappings.Share => DualShock4Button.Share,
            // ButtonMappings.TriggerRight => DualShock4Button.TriggerRight,
            // ButtonMappings.TriggerLeft => DualShock4Button.TriggerLeft,
            ButtonMappings.ShoulderRight => DualShock4Button.ShoulderRight,
            ButtonMappings.ShoulderLeft => DualShock4Button.ShoulderLeft,
            ButtonMappings.Triangle => DualShock4Button.Triangle,
            ButtonMappings.Circle => DualShock4Button.Circle,
            ButtonMappings.Cross => DualShock4Button.Cross,
            ButtonMappings.Ps => DualShock4SpecialButton.Ps,
            ButtonMappings.Touchpad => DualShock4SpecialButton.Touchpad,
            _ => DualShock4Button.Square
        };
}