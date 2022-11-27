using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.Mappings;

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
            GetMockParsedData(button);
            controllerMock.Verify(c => c.SetButtonState(mapping, true), Times.Once);
            controllerMock.Invocations.Clear();
        }

        [DataTestMethod]
        [DataRow("b12", DPadMappings.North)]
        [DataRow("b13", DPadMappings.South)]
        [DataRow("b14", DPadMappings.West)]
        [DataRow("b15", DPadMappings.East)]
        public void DpadCommandsAreExecuted(string button, DPadMappings mapping)
        {
            GetMockParsedData(button);
            controllerMock.Verify(c => c.SetDPadState(mapping), Times.Once);
            controllerMock.Invocations.Clear();
        }

        [DataTestMethod]
        [DataRow("a0", AxisMappings.LeftThumbX)]
        [DataRow("a1", AxisMappings.LeftThumbY)]
        [DataRow("a2", AxisMappings.RightThumbX)]
        [DataRow("a3", AxisMappings.RightThumbY)]
        public void AxisCommandsAreExecuted(string button, AxisMappings mapping)
        {
            GetMockParsedData(button);
            controllerMock.Verify(c => c.SetAxisState(mapping, 0), Times.Once);
            controllerMock.Invocations.Clear();
        }

        private void GetMockParsedData(string button)
        {
            string states = $"{button}:1;t:1";
            IDictionary<decimal, IEnumerable<IControllerCommand>> results = parser.GetStates(states);
            Assert.AreEqual(1, results.Count);
            List<IControllerCommand> entries = results[Convert.ToDecimal(1)].ToList();
            Assert.AreEqual(1, entries.Count);
            entries[0].ExecuteCommand(controller);
        }

    }
}