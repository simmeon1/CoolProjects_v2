using System.Diagnostics;
using Vigem_Common;
using Vigem_Common.Mappings;

namespace Vigem_ClassLibrary.Commands
{
    [DebuggerDisplay("{mapping}, {pressed}")]
    public class ButtonCommand : IControllerCommand
    {
        private readonly ButtonMappings mapping;
        private readonly bool pressed;

        public ButtonCommand(ButtonMappings mapping, bool pressed)
        {
            this.mapping = mapping;
            this.pressed = pressed;
        }

        public void ExecuteCommand(IController controller)
        {
            controller.SetButtonState(mapping, pressed);
        }
    }
}
