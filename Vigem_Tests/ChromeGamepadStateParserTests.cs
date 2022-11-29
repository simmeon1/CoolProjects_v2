using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_Common;
using Vigem_Common.Mappings;

namespace Vigem_Tests
{
    [TestClass]
    public class ChromeGamepadStateParserTests
    {
        private readonly ChromeGamepadStateParser parser = new();
        private Mock<IController> controllerMock;
        private IController controller;

        [TestInitialize]
        public void TestInitialize()
        {
            controllerMock = new Mock<IController>();
            controller = controllerMock.Object;
        }

        [TestMethod]
        public void ButtonsAndTimesAreCorrectlyGrouped()
        {
            string states = "b0:1;b1:1;t:1.2345~b1:0;t:2";
            IDictionary<decimal, IEnumerable<IControllerCommand>> results = parser.GetStates(states);
            Assert.AreEqual(2, results.Count);
            
            List<IControllerCommand> entries = results[Convert.ToDecimal(1.2345)].ToList();
            Assert.AreEqual(2, entries.Count);

            entries[0].ExecuteCommand(controller);
            controllerMock.Verify(c => c.SetButtonState(ButtonMappings.Cross, true), Times.Once);
            controllerMock.Invocations.Clear();

            entries[1].ExecuteCommand(controller);
            controllerMock.Verify(c => c.SetButtonState(ButtonMappings.Circle, true), Times.Once);
            controllerMock.Invocations.Clear();

            entries = results[Convert.ToDecimal(2)].ToList();
            entries[0].ExecuteCommand(controller);
            controllerMock.Verify(c => c.SetButtonState(ButtonMappings.Circle, false), Times.Once);
            controllerMock.Invocations.Clear();
        }

        [DataTestMethod]
        [DataRow("b0", ButtonMappings.Cross)]
        [DataRow("b1", ButtonMappings.Circle)]
        [DataRow("b2", ButtonMappings.Square)]
        [DataRow("b3", ButtonMappings.Triangle)]
        [DataRow("b4", ButtonMappings.ShoulderLeft)]
        [DataRow("b5", ButtonMappings.ShoulderRight)]
        [DataRow("b8", ButtonMappings.Share)]
        [DataRow("b9", ButtonMappings.Options)]
        [DataRow("b10", ButtonMappings.ThumbLeft)]
        [DataRow("b11", ButtonMappings.ThumbRight)]
        public void ButtonCommandsAreExecuted(string button, ButtonMappings mapping)
        {
            ExecuteMockCommandFromButton(button);
            controllerMock.Verify(c => c.SetButtonState(mapping, true), Times.Once);
            controllerMock.Invocations.Clear();
        }

        [DataTestMethod]
        [DataRow("b12", DPadMappings.Up)]
        [DataRow("b13", DPadMappings.Down)]
        [DataRow("b14", DPadMappings.Left)]
        [DataRow("b15", DPadMappings.Right)]
        public void DpadCommandsAreExecuted(string button, DPadMappings mapping)
        {
            ExecuteMockCommandFromButton(button);
            controllerMock.Verify(c => c.SetDPadState(mapping, true), Times.Once);
            controllerMock.Invocations.Clear();
        }

        [DataTestMethod]
        [DataRow("a0", AxisMappings.LeftThumbX)]
        [DataRow("a1", AxisMappings.LeftThumbY)]
        [DataRow("a2", AxisMappings.RightThumbX)]
        [DataRow("a3", AxisMappings.RightThumbY)]
        public void AxisCommandsAreExecuted(string button, AxisMappings mapping)
        {
            ExecuteMockCommandFromButton(button);
            controllerMock.Verify(c => c.SetAxisState(mapping, 255), Times.Once);
            controllerMock.Invocations.Clear();
        }
        [DataTestMethod]
        [DataRow("0", 128)]
        [DataRow("1", 255)]
        [DataRow("-1", 0)]
        [DataRow("0.5", 192)]
        [DataRow("0.55", 198)]
        [DataRow("-0.5", 64)]
        public void AxisValuesAreCorrectlyConverted(string value, int expected)
        {
            ExecuteMockCommandFromState($"a0:{value};t:1");
            controllerMock.Verify(c => c.SetAxisState(AxisMappings.LeftThumbX, Convert.ToByte(expected)), Times.Once);
            controllerMock.Invocations.Clear();
        }


        private void ExecuteMockCommandFromButton(string button)
        {
            ExecuteCommandFromMock($"{button}:1;t:1");
        }

        private void ExecuteMockCommandFromState(string state)
        {
            ExecuteCommandFromMock(state);
        }

        private void ExecuteCommandFromMock(string states)
        {
            IDictionary<decimal, IEnumerable<IControllerCommand>> results = parser.GetStates(states);
            Assert.AreEqual(1, results.Count);
            List<IControllerCommand> entries = results[Convert.ToDecimal(1)].ToList();
            Assert.AreEqual(1, entries.Count);
            entries[0].ExecuteCommand(controller);
        }
    }
}