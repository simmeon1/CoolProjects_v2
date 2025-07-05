using VigemLibrary.Mappings;

namespace VigemLibrary.Controllers;

public record ControllerState(
    IReadOnlyDictionary<AxisMappings, byte> axisStates,
    IReadOnlyDictionary<ButtonMappings, bool> buttonStates,
    IReadOnlyDictionary<DPadMappings, bool> dpadStates,
    IReadOnlyDictionary<TriggerMappings, byte> triggerStates
);