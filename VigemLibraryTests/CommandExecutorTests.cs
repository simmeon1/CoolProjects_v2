using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VigemLibrary;
using VigemLibrary.Commands;
using VigemLibrary.Controllers;
using VigemLibrary.SystemImplementations;

namespace VigemLibraryTests
{
    [TestClass]
    public class CommandExecutorTests
    {
        private Mock<IStopwatch> s;
        private Mock<IController> c;

        [TestInitialize]
        public void TestInitialize()
        {
            c = new Mock<IController>();
            s = new Mock<IStopwatch>();
        }

        [TestMethod]
        public void ExecutesAllCommandsOnTime()
        {
            List<int> stopwatchResults = new();
            List<int> commandResults = new();
            Mock<IControllerCommand> command1 = new();
            Mock<IControllerCommand> command2 = new();
            Mock<IControllerCommand> command3 = new();
            IDictionary<double, IEnumerable<IControllerCommand>> tsAndCmds 
                = new Dictionary<double, IEnumerable<IControllerCommand>>()
            {
                { 300,  new List<IControllerCommand> { command1.Object } },
                { 200,  new List<IControllerCommand>() },
                { 100,  new List<IControllerCommand> { command2.Object, command3.Object } },
            };

            IController controller = c.Object;
            command2.Setup(c => c.ExecuteCommand(controller)).Callback(() => commandResults.Add(0));
            command3.Setup(c => c.ExecuteCommand(controller)).Callback(() => commandResults.Add(1));
            command1.Setup(c => c.ExecuteCommand(controller)).Callback(() => commandResults.Add(2));

            s.Setup(s => s.Restart()).Callback(() => stopwatchResults.Add(0));
            s.Setup(s => s.WaitUntilTimestampReached(0)).Callback(() => stopwatchResults.Add(1));
            s.Setup(s => s.WaitUntilTimestampReached(100)).Callback(() => stopwatchResults.Add(2));
            s.Setup(s => s.WaitUntilTimestampReached(200)).Callback(() => stopwatchResults.Add(3));
            s.Setup(s => s.Stop()).Callback(() => stopwatchResults.Add(4));

            CommandExecutor executor = new(s.Object, controller);
            executor.ExecuteCommands(tsAndCmds);

            Assert.AreEqual(5, stopwatchResults.Count);
            Assert.AreEqual(0, stopwatchResults[0]);
            Assert.AreEqual(1, stopwatchResults[1]);
            Assert.AreEqual(2, stopwatchResults[2]);
            Assert.AreEqual(3, stopwatchResults[3]);
            Assert.AreEqual(4, stopwatchResults[4]);

            Assert.AreEqual(3, commandResults.Count);
            Assert.AreEqual(0, commandResults[0]);
            Assert.AreEqual(1, commandResults[1]);
            Assert.AreEqual(2, commandResults[2]);
        }
    }
}