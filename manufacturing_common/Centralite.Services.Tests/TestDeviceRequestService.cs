using Centralite.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestDeviceRequestService
    {
        private DeviceRequestService deviceRequestService = new DeviceRequestService();

        [TestMethod]
        public void TestClearProducerEvents()
        {
            bool deviceRequestCalled = false;
            deviceRequestService.OnDeviceRequest += delegate (ushort address)
            {
                deviceRequestCalled = true;
                return new DemoZigbeeDeviceBase() { Address = address };
            };

            deviceRequestService.ClearProducerEvents();

            Assert.IsFalse(deviceRequestCalled);
        }

        [TestMethod]
        public void TestRequestDevice()
        {
            bool deviceRequestCalled = false;
            deviceRequestService.OnDeviceRequest += delegate (ushort address)
            {
                deviceRequestCalled = true;
                return new DemoZigbeeDeviceBase() { Address = address };
            };

            var device = deviceRequestService.RequestDevice(0xFFFF);

            Assert.IsTrue(deviceRequestCalled);
            Assert.AreEqual(device.Address, 0xFFFF);
        }
    }
}
