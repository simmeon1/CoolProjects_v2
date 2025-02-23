using System.Diagnostics;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;

namespace VigemLibrary.Commands
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
