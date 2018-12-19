using Centralite.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestTestHostService
    {
        TestHostService testHostService = new TestHostService();

        [TestMethod]
        public void TestDeviceJoined()
        {
            ZigbeeDeviceBase testDevice = null;
            testHostService.OnDeviceJoined += delegate (ZigbeeDeviceBase device)
            {
                testDevice = device;
            };

            var createdDevice = new DemoZigbeeDeviceBase();

            testHostService.DeviceJoined(createdDevice);

            Assert.AreEqual(createdDevice, testDevice);
        }

        [TestMethod]
        public void TestDeviceLeft()
        {
            ZigbeeDeviceBase testDevice = null;
            testHostService.OnDeviceLeft += delegate (ZigbeeDeviceBase device)
            {
                testDevice = device;
            };

            var createdDevice = new DemoZigbeeDeviceBase();

            testHostService.DeviceLeft(createdDevice);

            Assert.AreEqual(createdDevice, testDevice);
        }

        [TestMethod]
        public void TestRequestDevices()
        {
            var deviceList = new List<ZigbeeDeviceBase>();
            testHostService.OnDevicesRequested += delegate ()
            {
                return deviceList;
            };

            var testDeviceList = testHostService.RequestDevices();

            Assert.AreEqual(deviceList, testDeviceList);
        }

        [TestMethod]
        public void TestClearTestEvents()
        {
            ZigbeeDeviceBase testDevice = null;
            testHostService.OnDeviceLeft += delegate (ZigbeeDeviceBase device)
            {
                testDevice = device;
            };

            testHostService.OnDeviceJoined += delegate (ZigbeeDeviceBase device)
            {
                testDevice = device;
            };

            testHostService.ClearTestEvents();

            var createdDevice = new DemoZigbeeDeviceBase();

            testHostService.DeviceLeft(createdDevice);

            Assert.AreEqual(testDevice, null);

            testHostService.DeviceJoined(createdDevice);

            Assert.AreEqual(testDevice, null);
        }

        [TestMethod]
        public void TestClearHostEvents()
        {
            var deviceList = new List<ZigbeeDeviceBase>();
            testHostService.OnDevicesRequested += delegate ()
            {
                return deviceList;
            };

            testHostService.ClearHostEvents();

            var testDeviceList = testHostService.RequestDevices();

            Assert.AreEqual(testDeviceList, null);
        }
    }
}
