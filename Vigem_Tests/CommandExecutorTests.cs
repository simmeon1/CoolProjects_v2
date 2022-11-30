using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.Commands;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common;
using Vigem_Common.Mappings;

namespace Vigem_Tests
{
    [TestClass]
    public class CommandExecutorTests
    {
        private Mock<IStopwatch> s;
        private Mock<IController> c;

        [TestInitialize]
        public void TestInitialize()
        {
            c = new();
            s = new();
        }

        [TestMethod]
        public void ExecutesAllCommandsOnTime()
        {
            List<int> stopwatchResults = new();
            List<int> commandResults = new();
            Mock<IControllerCommand> command1 = new();
            Mock<IControllerCommand> command2 = new();
            Mock<IControllerCommand> command3 = new();
            IDictionary<double, IEnumerable<IControllerCommand>> tsAndCmds = new Dictionary<double, IEnumerable<IControllerCommand>>()
            {
                { 3,  new List<IControllerCommand>() { command1.Object } },
                { 2,  new List<IControllerCommand>() { } },
                { 1,  new List<IControllerCommand>() { command2.Object, command3.Object } },
            };

            IController controller = c.Object;
            command2.Setup(c => c.ExecuteCommand(controller)).Callback(() => commandResults.Add(0));
            command3.Setup(c => c.ExecuteCommand(controller)).Callback(() => commandResults.Add(1));
            command1.Setup(c => c.ExecuteCommand(controller)).Callback(() => commandResults.Add(2));

            s.Setup(s => s.Reset()).Callback(() => stopwatchResults.Add(0));
            s.Setup(s => s.Start()).Callback(() => stopwatchResults.Add(1));
            s.Setup(s => s.Stop()).Callback(() => stopwatchResults.Add(2));
            s.SetupSequence(s => s.GetElapsedTotalMilliseconds()).Returns(0).Returns(1).Returns(2).Returns(3);

            CommandExecutor executor = new(s.Object);
            executor.ExecuteCommands(tsAndCmds, controller);

            Assert.AreEqual(3, stopwatchResults.Count);
            Assert.AreEqual(0, stopwatchResults[0]);
            Assert.AreEqual(1, stopwatchResults[1]);
            Assert.AreEqual(2, stopwatchResults[2]);

            Assert.AreEqual(3, commandResults.Count);
            Assert.AreEqual(0, commandResults[0]);
            Assert.AreEqual(1, commandResults[1]);
            Assert.AreEqual(2, commandResults[2]);
        }
    }
}