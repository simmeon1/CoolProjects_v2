using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using VigemLibrary.Controllers;
using VigemLibrary.Mappings;

namespace VigemControllers_Tests
{
    [TestClass]
    public class Xbox360ControllerTests
    {
        Mock<IXbox360Controller> controllerMock;
        Xbox360Controller c;

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
            controllerMock.Verify(cl => cl.SetAxisValue(Xbox360Axis.LeftThumbX, 0), Times.Once);
        }

        [TestMethod]
        public void SetsLeftThumbYAxisValue()
        {
            c.SetAxisState(AxisMappings.LeftThumbY, byte.MaxValue);
            controllerMock.Verify(cl => cl.SetAxisValue(Xbox360Axis.LeftThumbY, short.MinValue), Times.Once);
        }

        [TestMethod]
        public void SetsRightThumbXAxisValue()
        {
            c.SetAxisState(AxisMappings.RightThumbX, 129);
            controllerMock.Verify(cl => cl.SetAxisValue(Xbox360Axis.RightThumbX, 258), Times.Once);
        }

        [TestMethod]
        public void SetsRightThumbYAxisValueWithPositiveValue()
        {
            c.SetAxisState(AxisMappings.RightThumbY, byte.MinValue);
            controllerMock.Verify(cl => cl.SetAxisValue(Xbox360Axis.RightThumbY, short.MaxValue), Times.Once);
        }

        [TestMethod]
        public void SetsRightThumbYAxisValueWithNegativeValue()
        {
            c.SetAxisState(AxisMappings.RightThumbY, 127);
            controllerMock.Verify(cl => cl.SetAxisValue(Xbox360Axis.RightThumbY, 256), Times.Once);
        }

        [TestMethod]
        public void PressesThumbRight()
        {
            c.SetButtonState(ButtonMappings.ThumbRight, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.RightThumb, true), Times.Once);
        }

        [TestMethod]
        public void PressesThumbLeft()
        {
            c.SetButtonState(ButtonMappings.ThumbLeft, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.LeftThumb, true), Times.Once);
        }

        [TestMethod]
        public void PressesOptions()
        {
            c.SetButtonState(ButtonMappings.Options, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Start, true), Times.Once);
        }

        [TestMethod]
        public void PressesShare()
        {
            c.SetButtonState(ButtonMappings.Share, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Back, true), Times.Once);
        }

        [TestMethod]
        public void PressesShoulderRight()
        {
            c.SetButtonState(ButtonMappings.ShoulderRight, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.RightShoulder, true), Times.Once);
        }

        [TestMethod]
        public void PressesShoulderLeft()
        {
            c.SetButtonState(ButtonMappings.ShoulderLeft, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.LeftShoulder, true), Times.Once);
        }

        [TestMethod]
        public void PressesTriangle()
        {
            c.SetButtonState(ButtonMappings.Triangle, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Y, true), Times.Once);
        }

        [TestMethod]
        public void PressesCircle()
        {
            c.SetButtonState(ButtonMappings.Circle, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.B, true), Times.Once);
        }

        [TestMethod]
        public void PressesCross()
        {
            c.SetButtonState(ButtonMappings.Cross, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.A, true), Times.Once);
        }

        [TestMethod]
        public void PressesSquare()
        {
            c.SetButtonState(ButtonMappings.Square, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.X, true), Times.Once);
        }

        [TestMethod]
        public void PressesUp()
        {
            c.SetDPadState(DPadMappings.Up, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Up, true), Times.Once);
        }

        [TestMethod]
        public void PressesDown()
        {
            c.SetDPadState(DPadMappings.Down, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Down, true), Times.Once);
        }

        [TestMethod]
        public void PressesLeft()
        {
            c.SetDPadState(DPadMappings.Left, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Left, true), Times.Once);
        }

        [TestMethod]
        public void PressesRight()
        {
            c.SetDPadState(DPadMappings.Right, true);
            controllerMock.Verify(cl => cl.SetButtonState(Xbox360Button.Right, true), Times.Once);
        }
    }
}