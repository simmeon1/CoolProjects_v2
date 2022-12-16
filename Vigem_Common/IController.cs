using Vigem_Common.Mappings;

namespace Vigem_Common
{
    public interface IController
    {
        void Connect();
        void Disconnect();
        void SetDPadState(DPadMappings direction, bool pressed);
        void SetButtonState(ButtonMappings button, bool pressed);
        void SetAxisState(AxisMappings axis, byte value);
        void SetTriggerState(TriggerMappings trigger, byte value);
    }
}