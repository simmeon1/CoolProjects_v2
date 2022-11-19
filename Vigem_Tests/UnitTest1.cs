using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vigem_ClassLibrary;

namespace Vigem_Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestHaveCjSwimInCircles()
        {
            await Task.Delay(3000);
            Ds4Controller controller = new();
            Ds4ControllerUser controllerUser = new(controller);
            // await controller.PressButton(ButtonMappings.Options);
            controllerUser.HoldDPad(DPadMappings.West);
            controllerUser.HoldButton(ButtonMappings.Cross);
            controllerUser.HoldStick(AxisMappings.LeftThumbX, byte.MaxValue);
        }
    }
}