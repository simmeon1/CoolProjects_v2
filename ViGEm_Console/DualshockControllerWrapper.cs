using System.Threading.Tasks;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace ViGEm_Common
{
    public class DualshockControllerWrapper
    {
        private readonly IDualShock4Controller controller;
        private HtmlControllerState lastState = new();
        public DualshockControllerWrapper(IDualShock4Controller controller)
        {
            this.controller = controller;
        }

        public void SubmitReport()
        {
            controller.SubmitReport();
        }

        public void SetStateFromHtmlControllerState(HtmlControllerState state)
        {
            if (state.A0 != lastState.A0) controller.SetAxisValue(0, state.A0);
            if (state.A1 != lastState.A1) controller.SetAxisValue(1, state.A1);
            if (state.A2 != lastState.A2) controller.SetAxisValue(2, state.A2);
            if (state.A3 != lastState.A3) controller.SetAxisValue(3, state.A3);
            if (state.B0 != lastState.B0) controller.SetButtonState(DualShock4Button.Cross, state.B0);
            if (state.B1 != lastState.B1) controller.SetButtonState(DualShock4Button.Circle, state.B1);
            if (state.B2 != lastState.B2) controller.SetButtonState(DualShock4Button.Square, state.B2);
            if (state.B3 != lastState.B3) controller.SetButtonState(DualShock4Button.Triangle, state.B3);
            if (state.B4 != lastState.B4) controller.SetButtonState(DualShock4Button.ShoulderLeft, state.B4);
            if (state.B5 != lastState.B5) controller.SetButtonState(DualShock4Button.ShoulderRight, state.B5);
            if (state.B6 != lastState.B6) controller.SetButtonState(DualShock4Button.TriggerLeft, state.B6);
            if (state.B7 != lastState.B7) controller.SetButtonState(DualShock4Button.TriggerRight, state.B7);
            if (state.B8 != lastState.B8) controller.SetButtonState(DualShock4Button.Share, state.B8);
            if (state.B9 != lastState.B9) controller.SetButtonState(DualShock4Button.Options, state.B9);
            if (state.B1 != lastState.B1) controller.SetButtonState(DualShock4Button.ThumbLeft, state.B10);
            if (state.B1 != lastState.B1) controller.SetButtonState(DualShock4Button.ThumbRight, state.B11);
            if (state.B12) controller.SetDPadDirection(DualShock4DPadDirection.North);
            else if (state.B13) controller.SetDPadDirection(DualShock4DPadDirection.South);
            else if (state.B14) controller.SetDPadDirection(DualShock4DPadDirection.West);
            else if (state.B15) controller.SetDPadDirection(DualShock4DPadDirection.East);
            else controller.SetDPadDirection(DualShock4DPadDirection.None);
            // Controller.SetButtonState(DualShock4Button.ps, state.b16);
            // Controller.SetButtonState(DualShock4Button.touch, state.b17);
            // controller.SubmitReport();
            lastState = state;
        }

        public void SetButtonState(DualShock4Button button, bool pressed)
        {
            controller.SetButtonState(button, pressed);
        }

        public void SetDPadDirection(DualShock4DPadDirection direction)
        {
            controller.SetDPadDirection(direction);
        }

        public void SetAxisValue(DualShock4Axis axis, byte value)
        {
            controller.SetAxisValue(axis, value);
        }

        public void SetAxisValue(int index, short value)
        {
            controller.SetAxisValue(index, value);
        }

        public async Task PressButton(
            DualShock4Button button,
            int delayBeforeRelease = 500,
            int delayAfterRelease = 500
        )
        {
            controller.SetButtonState(button, true);
            await Task.Delay(delayBeforeRelease);
            controller.SetButtonState(button, false);
            await Task.Delay(delayAfterRelease);
        }

        public async Task PushStickUp(DualShock4Axis axis, int delayBeforeRelease = 500, int delayAfterRelease = 500)
        {
            controller.SetAxisValue(axis, 255);
            await Task.Delay(delayBeforeRelease);
            controller.SetAxisValue(axis, 128);
            await Task.Delay(delayAfterRelease);
        }

        public async Task PushStickDown(DualShock4Axis axis, int delayBeforeRelease = 500, int delayAfterRelease = 500)
        {
            controller.SetAxisValue(axis, 0);
            await Task.Delay(delayBeforeRelease);
            controller.SetAxisValue(axis, 128);
            await Task.Delay(delayAfterRelease);
        }

        public async Task PressDpad(
            DualShock4DPadDirection dpad,
            int delayBeforeRelease = 500,
            int delayAfterRelease = 500
        )
        {
            controller.SetDPadDirection(dpad);
            await Task.Delay(delayBeforeRelease);
            controller.SetDPadDirection(DualShock4DPadDirection.None);
            await Task.Delay(delayAfterRelease);
        }
    }
}