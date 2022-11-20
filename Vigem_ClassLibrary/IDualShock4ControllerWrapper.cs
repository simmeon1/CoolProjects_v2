using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;

namespace Vigem_ClassLibrary
{
    public interface IDualShock4ControllerWrapper
    {
        void Connect();
        void Disconnect();
        void SetDPadDirection(DualShock4DPadDirection direction);
        void SetButtonState(DualShock4Button button, bool pressed);
        void SetAxisValue(DualShock4Axis axis, byte value);
    }
}