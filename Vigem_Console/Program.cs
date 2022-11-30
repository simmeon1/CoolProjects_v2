using Nefarius.ViGEm.Client.Targets;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common.Mappings;
using VigemControllers_ClassLibrary;

namespace Vigem_Console
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Delayer delayer = new();
            ControllerCreator creator = new();

            IXbox360Controller created360Controller = creator.GetXbox360Controller();
            Xbox360Controller c360 = new(created360Controller);

            IDualShock4Controller createdDs4Controller = creator.GetDualShock4Controller();
            Dualshock4Controller cds4 = new(createdDs4Controller);

            ControllerUser user360 = new(c360, delayer, 500);
            user360.Connect();
            ControllerUser userDs4 = new(cds4, delayer, 500);
            userDs4.Connect();

            var x = 1;
            //await delayer.Delay(1000);
            //foreach (ButtonMappings mapping in Enum.GetValues<ButtonMappings>())
            //{
            //    await controller.PressButton(mapping);
            //}

            //foreach (AxisMappings mapping in Enum.GetValues<AxisMappings>())
            //{
            //    await controller.PressStick(mapping, byte.MaxValue);
            //}

            //foreach (DPadMappings mapping in Enum.GetValues<DPadMappings>())
            //{
            //    await controller.PressDPad(mapping, byte.MaxValue);
            //}
            //controller.Disconnect();


            //Delayer delayer = new();
            //ControllerUser controller = new(new Dualshock4Controller(), delayer, 500);
            //controller.Connect();
            //await delayer.Delay(1000);
            //foreach (ButtonMappings mapping in Enum.GetValues<ButtonMappings>())
            //{
            //    await controller.PressButton(mapping);
            //}

            //foreach (AxisMappings mapping in Enum.GetValues<AxisMappings>())
            //{
            //    await controller.PressStick(mapping, byte.MaxValue);
            //}

            //foreach (DPadMappings mapping in Enum.GetValues<DPadMappings>())
            //{
            //    await controller.PressDPad(mapping, byte.MaxValue);
            //}
            //controller.Disconnect();
        }
    }
}