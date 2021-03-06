using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using static System.Byte;

namespace AutoInput
{
    public class DualshockControllerWrapper
    {
        private readonly IDualShock4Controller controller;
        public DualshockControllerWrapper()
        {
            controller = new ViGEmClient().CreateDualShock4Controller();
            controller.AutoSubmitReport = false;   
        }
        
        public void Connect()
        {
            controller.Connect();
        }
        
        public void Disconnect()
        {
            controller.Disconnect();
        }

        public void SetState(ControllerState controllerState)
        {
            controller.SetAxisValue(0, controllerState.A0);
            controller.SetAxisValue(1, controllerState.A1);
            controller.SetAxisValue(2, controllerState.A2);
            controller.SetAxisValue(3, controllerState.A3);
            controller.SetButtonState(DualShock4Button.Cross, controllerState.B0);
            controller.SetButtonState(DualShock4Button.Circle, controllerState.B1);
            controller.SetButtonState(DualShock4Button.Square, controllerState.B2);
            controller.SetButtonState(DualShock4Button.Triangle, controllerState.B3);
            controller.SetButtonState(DualShock4Button.ShoulderLeft, controllerState.B4);
            controller.SetButtonState(DualShock4Button.ShoulderRight, controllerState.B5);
            controller.SetSliderValue(DualShock4Slider.LeftTrigger, GetSliderValue(controllerState.B6));
            controller.SetSliderValue(DualShock4Slider.RightTrigger, GetSliderValue(controllerState.B7));
            controller.SetButtonState(DualShock4Button.Share, controllerState.B8);
            controller.SetButtonState(DualShock4Button.Options, controllerState.B9);
            controller.SetButtonState(DualShock4Button.ThumbLeft, controllerState.B10);
            controller.SetButtonState(DualShock4Button.ThumbRight, controllerState.B11);
            if (controllerState.B12) controller.SetDPadDirection(DualShock4DPadDirection.North);
            else if (controllerState.B13) controller.SetDPadDirection(DualShock4DPadDirection.South);
            else if (controllerState.B14) controller.SetDPadDirection(DualShock4DPadDirection.West);
            else if (controllerState.B15) controller.SetDPadDirection(DualShock4DPadDirection.East);
            else controller.SetDPadDirection(DualShock4DPadDirection.None);
            // Controller.SetButtonState(DualShock4Button.ps, state.b16);
            // Controller.SetButtonState(DualShock4Button.touch, state.b17);
            controller.SubmitReport();
        }

        private static byte GetSliderValue(bool sliderIsPressed)
        {
            return sliderIsPressed ? MaxValue : MinValue;
        }

        public void SetButtonState(DualShock4Button button, bool pressed)
        {
            controller.SetButtonState(button, pressed);
            controller.SubmitReport();
        }

        public void SetDPadDirection(DualShock4DPadDirection direction)
        {
            controller.SetDPadDirection(direction);
            controller.SubmitReport();
        }

        public void SetAxisValue(DualShock4Axis axis, byte value)
        {
            controller.SetAxisValue(axis, value);
            controller.SubmitReport();
        }

        public void SetAxisValue(int index, short value)
        {
            controller.SetAxisValue(index, value);
            controller.SubmitReport();
        }
    }
}