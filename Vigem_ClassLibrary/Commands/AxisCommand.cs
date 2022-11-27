using System.Diagnostics;
using Vigem_ClassLibrary.Mappings;

namespace Vigem_ClassLibrary.Commands
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
