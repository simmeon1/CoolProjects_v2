using System.Diagnostics;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;

namespace VigemLibrary.Commands
{
    [DebuggerDisplay("{mapping}, {value}")]
    public class TriggerCommand : IControllerCommand
    {
        private readonly TriggerMappings mapping;
        private readonly byte value;

        public TriggerCommand(TriggerMappings mapping, byte value)
        {
            this.mapping = mapping;
            this.value = value;
        }

        public void ExecuteCommand(IController controller)
        {
            controller.SetTriggerState(mapping, value);
        }
    }
}
