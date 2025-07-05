using VigemLibrary.Controllers;
using VigemLibrary.Mappings;
using VigemLibrary.SystemImplementations;

namespace VigemLibrary;

public class StopwatchControllerUser {
    private readonly IController controller;
    private readonly int delayAfterSet;
    private readonly int pressLength;
    private readonly IStopwatch stopwatch;

    // Changing away form zero could break other usages like jump rope
    public StopwatchControllerUser(
        IController controller,
        IStopwatch stopwatch,
        int pressLength,
        int delayAfterSet = 0
    ) {
        this.controller = controller;
        this.stopwatch = stopwatch;
        this.pressLength = pressLength;
        this.delayAfterSet = delayAfterSet;
        this.stopwatch.Restart();
    }

    public void Connect() {
        controller.Connect();
    }

    public void Disconnect() {
        controller.Disconnect();
    }

    public void HoldButton(ButtonMappings button) {
        SetButtonState(button, true);
    }

    public void ReleaseButton(ButtonMappings button) {
        SetButtonState(button, false);
    }

    public void PressButton(ButtonMappings button, int? holdDuration = null) {
        HoldButton(button);
        Wait(holdDuration);
        ReleaseButton(button);
    }

    public void SetButtonState(ButtonMappings button, bool pressed) {
        controller.SetButtonState(button, pressed);
        Wait(delayAfterSet);
    }

    public void HoldDPad(DPadMappings direction) {
        SetDPadDirection(direction, true);
    }

    public void ReleaseDPad(DPadMappings direction) {
        SetDPadDirection(direction, false);
    }

    public void PressDPad(DPadMappings direction, int? delay = null) {
        HoldDPad(direction);
        Wait(delay);
        ReleaseDPad(direction);
    }

    public void SetDPadDirection(DPadMappings direction, bool pressed) {
        controller.SetDPadState(direction, pressed);
        Wait(delayAfterSet);
    }

    public void PushAxis(AxisMappings axis) {
        SetAxis(axis, byte.MaxValue);
    }

    public void PullAxis(AxisMappings axis) {
        SetAxis(axis, byte.MinValue);
    }

    public void ResetAxis(AxisMappings axis) {
        SetAxis(axis, 128);
    }

    public void SetAxis(AxisMappings axis, byte value) {
        controller.SetAxisState(axis, value);
        Wait(delayAfterSet);
    }

    public void HoldTrigger(TriggerMappings trigger) {
        SetTrigger(trigger, byte.MaxValue);
    }

    public void ReleaseTrigger(TriggerMappings trigger) {
        SetTrigger(trigger, byte.MinValue);
    }

    public void PressTrigger(TriggerMappings trigger, int? delay = null) {
        HoldTrigger(trigger);
        Wait(delay);
        ReleaseTrigger(trigger);
    }

    public void SetTrigger(TriggerMappings trigger, byte value) {
        controller.SetTriggerState(trigger, value);
        Wait(delayAfterSet);
    }

    public ControllerState GetState() => controller.GetState();

    private void Wait(int? delay) {
        stopwatch.Wait(delay ?? pressLength);
    }
}