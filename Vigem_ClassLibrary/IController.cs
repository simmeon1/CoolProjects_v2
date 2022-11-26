using Vigem_ClassLibrary.Mappings;

namespace Vigem_ClassLibrary
{
    public interface IController
    {
        void Connect();
        void Disconnect();
        void SetDPadState(DPadMappings direction);
        void SetButtonState(ButtonMappings button, bool pressed);
        void SetAxisState(AxisMappings axis, byte value);
    }
}