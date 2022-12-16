﻿using System.Diagnostics;
using Vigem_Common;
using Vigem_Common.Mappings;

namespace Vigem_ClassLibrary.Commands
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
