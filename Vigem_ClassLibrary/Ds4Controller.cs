using System;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace Vigem_ClassLibrary
{
    public class Ds4Controller: IDualShock4ControllerWrapper
    {
        private readonly IDualShock4Controller controller;

        public Ds4Controller()
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

        public void SetDPadDirection(DualShock4DPadDirection direction)
        {
            controller.SetDPadDirection(direction);
        }

        public void SetButtonState(DualShock4Button button, bool pressed)
        {
            controller.SetButtonState(button, pressed);
        }

        public void SetAxisValue(DualShock4Axis axis, byte value)
        {
            controller.SetAxisValue(axis, value);
        }
    }
}