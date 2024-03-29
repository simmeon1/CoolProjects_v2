using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Vigem_ClassLibrary;
using Vigem_ClassLibrary.SystemImplementations;
using Vigem_Common;
using Vigem_Common.Mappings;

namespace Vigem_Tests
{
    [TestClass]
    public class DelayerControllerUserTests
    {
        private Mock<IController> controller;
        private Mock<IDelayer> delayer;
        private DelayerControllerUser user;
        private const int defaultPressTime = 200;

        [TestInitialize]
        public void TestInitialize()
        {
            controller = new();
            delayer = new();
            user = new(controller.Object, delayer.Object, defaultPressTime);
        }

        [TestMethod]
        public void ControllerConnectsSuccessfully()
        {
            user.Connect();
            controller.Verify(c => c.Connect(), Times.Once);
        }

        [TestMethod]
        public void ControllerDisconnectsSuccessfully()
        {
            user.Disconnect();
            controller.Verify(c => c.Disconnect(), Times.Once);
        }

        [TestMethod]
        public async Task ButtonIsPressedFor200Seconds()
        {
            ButtonMappings button = ButtonMappings.ThumbLeft;
            await user.PressButton(button);
            controller.Verify(c => c.SetButtonState(button, true), Times.Once);
            delayer.Verify(d => d.Delay(defaultPressTime), Times.Once);
            controller.Verify(c => c.SetButtonState(button, false), Times.Once);
        }

        [TestMethod]
        public void ButtonIsHeld()
        {
            ButtonMappings button = ButtonMappings.ThumbLeft;
            user.HoldButton(button);
            controller.Verify(c => c.SetButtonState(button, true), Times.Once);
        }

        [TestMethod]
        public void ButtonIsReleased()
        {
            ButtonMappings button = ButtonMappings.ThumbLeft;
            user.ReleaseButton(button);
            controller.Verify(c => c.SetButtonState(button, false), Times.Once);
        }

        [TestMethod]
        public async Task DpadIsPressedFor300Seconds()
        {
            await user.PressDPad(DPadMappings.Up, 300);
            controller.Verify(c => c.SetDPadState(DPadMappings.Up, true), Times.Once);
            delayer.Verify(d => d.Delay(300), Times.Once);
            controller.Verify(c => c.SetDPadState(DPadMappings.Up, false), Times.Once);
        }

        [TestMethod]
        public void DpadIsHeld()
        {
            DPadMappings mapping = DPadMappings.Up;
            user.HoldDPad(mapping);
            controller.Verify(c => c.SetDPadState(mapping, true), Times.Once);
        }

        [TestMethod]
        public void DpadIsReleased()
        {
            DPadMappings mapping = DPadMappings.Up;
            user.ReleaseDPad(mapping);
            controller.Verify(c => c.SetDPadState(mapping, false), Times.Once);
        }

        [TestMethod]
        public async Task AxisIsMoved()
        {
            AxisMappings mapping = AxisMappings.LeftThumbX;
            byte axisValue = byte.MaxValue;
            await user.PressStick(mapping, axisValue, null);
            controller.Verify(c => c.SetAxisState(mapping, axisValue), Times.Once);
            delayer.Verify(d => d.Delay(defaultPressTime), Times.Once);
            controller.Verify(c => c.SetAxisState(mapping, 128), Times.Once);
        }

        [TestMethod]
        public void AxisIsHeld()
        {
            AxisMappings mapping = AxisMappings.LeftThumbX;
            byte axisValue = byte.MaxValue;
            user.HoldStick(mapping, axisValue);
            controller.Verify(c => c.SetAxisState(mapping, axisValue), Times.Once);
        }

        [TestMethod]
        public void AxisIsReleased()
        {
            AxisMappings mapping = AxisMappings.LeftThumbX;
            user.ReleaseStick(mapping);
            controller.Verify(c => c.SetAxisState(mapping, 128), Times.Once);
        }
    }
}