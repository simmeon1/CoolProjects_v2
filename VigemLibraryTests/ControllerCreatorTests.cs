using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nefarius.ViGEm.Client.Targets;
using VigemLibrary.Controllers;

namespace VigemControllers_Tests
{
    [TestClass]
    public class ControllerCreatorTests
    {
        private ControllerCreator creator;

        [TestInitialize]
        public void TestInitialize()
        {
            creator = new ControllerCreator();
        }

        [TestMethod]
        public void Creates360Controller()
        {
            Assert.IsInstanceOfType(creator.GetXbox360Controller(), typeof(IXbox360Controller));
        }

        [TestMethod]
        public void CreatesDs4Controller()
        {
            Assert.IsInstanceOfType(creator.GetDualShock4Controller(), typeof(IDualShock4Controller));
        }
    }
}