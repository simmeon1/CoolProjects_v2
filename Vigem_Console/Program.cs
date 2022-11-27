using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Mappings;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Controllers;

namespace Vigem_Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Delayer delayer = new();
            ControllerUser controller = new(new Dualshock4Controller(), delayer, 500);
            controller.Connect();
            await delayer.Delay(1000);
            foreach (ButtonMappings mapping in Enum.GetValues<ButtonMappings>())
            {
                await controller.PressButton(mapping);
            }

            foreach (AxisMappings mapping in Enum.GetValues<AxisMappings>())
            {
                await controller.PressStick(mapping, byte.MaxValue);
            }

            foreach (DPadMappings mapping in Enum.GetValues<DPadMappings>())
            {
                await controller.PressDPad(mapping, byte.MaxValue);
            }
            controller.Disconnect();
        }
    }
}