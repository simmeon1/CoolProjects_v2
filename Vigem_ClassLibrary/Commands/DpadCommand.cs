using System.Diagnostics;
using Vigem_ClassLibrary.Mappings;

namespace Vigem_ClassLibrary.Commands
{
    [DebuggerDisplay("{mapping}")]
    public class DpadCommand : IControllerCommand
    {
        private readonly DPadMappings mapping;
        public DpadCommand(DPadMappings mapping)
        {
            this.mapping = mapping;
        }

        public void ExecuteCommand(IController controller)
        {
            controller.SetDPadState(mapping);
        }
    }
}
