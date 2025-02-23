using System.Diagnostics;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;

namespace VigemLibrary.Commands
{
    [DebuggerDisplay("{mapping}, {pressed}")]
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
