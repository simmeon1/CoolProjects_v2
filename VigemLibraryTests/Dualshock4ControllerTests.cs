using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;

namespace VigemControllers_Tests
{
    [TestClass]
    public class Dualshock4ControllerTests
    {
        Mock<IDualShock4Controller> controllerMock;
        Dualshock4Controller c;

        [TestInitialize]
        public void TestInitialize()
        {
            controllerMock = new();
            c = new(controllerMock.Object);
        }

        [TestMethod]
        public void Connects()
        {
            c.Connect();
            controllerMock.Verify(cl => cl.Connect(), Times.Once);
        }

        [TestMethod]
        public void Disconnects()
        {
            c.Disconnect();
            controllerMock.Verify(cl => cl.Disconnect(), Times.Once);
        }

        [TestMethod]
        public void SetsLeftThumbXAxisValue()
        {
            c.SetAxisState(AxisMappings.LeftThumbX, 128);
            controllerMock.Verify(cl => cl.SetAxisValue(DualShock4Axis.LeftThumbX, 128), Times.Once);
        }

        [TestMethod]
        public void SetsLeftThumbYAxisValue()
        {
            c.SetAxisState(AxisMappings.LeftThumbY, byte.MaxValue);
            controllerMock.Verify(cl => cl.SetAxisValue(DualShock4Axis.LeftThumbY, byte.MaxValue), Times.Once);
        }

        [TestMethod]
        public void SetsRightThumbXAxisValue()
        {
            c.SetAxisState(AxisMappings.RightThumbX, byte.MinValue);
            controllerMock.Verify(cl => cl.SetAxisValue(DualShock4Axis.RightThumbX, byte.MinValue), Times.Once);
        }

        [TestMethod]
        public void SetsRightThumbYAxisValue()
        {
            c.SetAxisState(AxisMappings.RightThumbY, 129);
            controllerMock.Verify(cl => cl.SetAxisValue(DualShock4Axis.RightThumbY, 129), Times.Once);
        }

        [TestMethod]
        public void PressesThumbRight()
        {
            c.SetButtonState(ButtonMappings.ThumbRight, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.ThumbRight, true), Times.Once);
        }

        [TestMethod]
        public void PressesThumbLeft()
        {
            c.SetButtonState(ButtonMappings.ThumbLeft, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.ThumbLeft, true), Times.Once);
        }

        [TestMethod]
        public void PressesOptions()
        {
            c.SetButtonState(ButtonMappings.Options, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.Options, true), Times.Once);
        }

        [TestMethod]
        public void PressesShare()
        {
            c.SetButtonState(ButtonMappings.Share, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.Share, true), Times.Once);
        }

        [TestMethod]
        public void PressesShoulderRight()
        {
            c.SetButtonState(ButtonMappings.ShoulderRight, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.ShoulderRight, true), Times.Once);
        }

        [TestMethod]
        public void PressesShoulderLeft()
        {
            c.SetButtonState(ButtonMappings.ShoulderLeft, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.ShoulderLeft, true), Times.Once);
        }

        [TestMethod]
        public void PressesTriangle()
        {
            c.SetButtonState(ButtonMappings.Triangle, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.Triangle, true), Times.Once);
        }

        [TestMethod]
        public void PressesCircle()
        {
            c.SetButtonState(ButtonMappings.Circle, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.Circle, true), Times.Once);
        }

        [TestMethod]
        public void PressesCross()
        {
            c.SetButtonState(ButtonMappings.Cross, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.Cross, true), Times.Once);
        }

        [TestMethod]
        public void PressesSquare()
        {
            c.SetButtonState(ButtonMappings.Square, true);
            controllerMock.Verify(cl => cl.SetButtonState(DualShock4Button.Square, true), Times.Once);
        }

        [TestMethod]
        public void SetsDpadNone()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Up, false);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.None), Times.Once);
        }

        [TestMethod]
        public void SetsDpadNorthwest()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Left, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.Northwest), Times.Once);
        }

        [TestMethod]
        public void SetsDpadWest()
        {
            c.SetDPadState(DPadMappings.Left, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.West), Times.Once);
        }

        [TestMethod]
        public void SetsDpadSouthwest()
        {
            c.SetDPadState(DPadMappings.Down, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.South), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Left, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.Southwest), Times.Once);
        }

        [TestMethod]
        public void SetsDpadSouth()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Down, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.South), Times.Once);
        }

        [TestMethod]
        public void SetsDpadSoutheast()
        {
            c.SetDPadState(DPadMappings.Down, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.South), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Right, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.Southeast), Times.Once);
        }

        [TestMethod]
        public void SetsDpadEast()
        {
            c.SetDPadState(DPadMappings.Right, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.East), Times.Once);
        }

        [TestMethod]
        public void SetsDpadNortheast()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Right, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.Northeast), Times.Once);
        }

        [TestMethod]
        public void SetsDpadNorthFromNortheast()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Right, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.Northeast), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Right, false);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
        }

        [TestMethod]
        public void SetsDpadEastFromNortheast()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Right, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.Northeast), Times.Once);
            ClearInvocations();

            c.SetDPadState(DPadMappings.Up, false);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.East), Times.Once);
        }

        [TestMethod]
        public void SetsDpadNorth()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetDPadDirection(DualShock4DPadDirection.North), Times.Once);
        }

        private void ClearInvocations()
        {
            controllerMock.Invocations.Clear();
        }
    }
}