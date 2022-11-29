using System.Diagnostics;
using Vigem_Common;
using Vigem_Common.Mappings;

namespace Vigem_ClassLibrary.Commands
{
    [DebuggerDisplay("{mapping}, {value}")]
    public class DpadCommand : IControllerCommand
    {
        private readonly DPadMappings mapping;
        private readonly bool pressed;

        public DpadCommand(DPadMappings mapping, bool pressed)
        {
            this.mapping = mapping;
            this.pressed = pressed;
        }

        public void ExecuteCommand(IController controller)
        {
            controller.SetDPadState(mapping, pressed);
        }
    }
}
