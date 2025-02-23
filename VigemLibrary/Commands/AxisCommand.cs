using System.Diagnostics;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;

namespace VigemLibrary.Commands
{
    [DebuggerDisplay("{mapping}, {value}")]
    public class AxisCommand : IControllerCommand
    {
        private readonly AxisMappings mapping;
        private readonly byte value;

        public AxisCommand(AxisMappings mapping, byte value)
        {
            this.mapping = mapping;
            this.value = value;
        }

        public void ExecuteCommand(IController controller)
        {
            controller.SetAxisState(mapping, value);
        }
    }
}
