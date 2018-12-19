using Centralite.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestTestFinalizeService
    {
        TestFinalizeService testFinalizeService;

        [TestInitialize]
        public void Setup()
        {
            testFinalizeService = new TestFinalizeService();
        }

        [TestMethod]
        public void TestClearEvents()
        {
            ZigbeeDeviceBase validationDevice = default(ZigbeeDeviceBase);
            int? validationTesterId = default(int?);
            int validationNetworkColorId = default(int);
            int validationProductionSiteId = default(int);
            int? validationSupervisorId = default(int?);

            testFinalizeService.OnLabelPrinted += delegate (ZigbeeDeviceBase device, int testerId, int networkColorId, int productionSiteId)
            {
                validationDevice = device;
                validationTesterId = testerId;
                validationNetworkColorId = networkColorId;
                validationProductionSiteId = productionSiteId;
            };

            testFinalizeService.OnSerialNumberGenerated += delegate (ZigbeeDeviceBase device, int? testerId, int? supervisorId)
            {
                validationDevice = device;
                validationTesterId = testerId;
                validationSupervisorId = supervisorId;
            };

            testFinalizeService.OnSerialNumberUpdated += delegate (ZigbeeDeviceBase device, int? testerId)
            {
                validationDevice = device;
                validationTesterId = testerId;
            };

            var testDevice = new DemoZigbeeDeviceBase();
            var testTesterId = 9;
            var testNetworkColorId = 99;
            var testProductionSiteId = 999;
            var testSupervisorId = 99;

            testFinalizeService.ClearEvents();

            testFinalizeService.LabelPrinted(testDevice, testTesterId, testNetworkColorId, testProductionSiteId);
            testFinalizeService.SerialNumberGenerated(testDevice, testTesterId, testSupervisorId);
            testFinalizeService.SerialNumberUpdated(testDevice, testTesterId);

            Assert.AreEqual(validationDevice, default(ZigbeeDeviceBase));
            Assert.AreEqual(validationTesterId, default(int?));
            Assert.AreEqual(validationNetworkColorId, default(int));
            Assert.AreEqual(validationProductionSiteId, default(int));
            Assert.AreEqual(validationSupervisorId, default(int?));
        }

        [TestMethod]
        public void TestLabelPrinted()
        {
            ZigbeeDeviceBase validationDevice = null;
            int validationTesterId = 0;
            int validationNetworkColorId = 0;
            int validationProductionSiteId = 0;

            testFinalizeService.OnLabelPrinted += delegate (ZigbeeDeviceBase device, int testerId, int networkColorId, int productionSiteId)
            {
                validationDevice = device;
                validationTesterId = testerId;
                validationNetworkColorId = networkColorId;
                validationProductionSiteId = productionSiteId;
            };

            var testDevice = new DemoZigbeeDeviceBase();
            var testTesterId = 9;
            var testNetworkColorId = 99;
            var testProductionSiteId = 999;

            testFinalizeService.LabelPrinted(testDevice, testTesterId, testNetworkColorId, testProductionSiteId);

            Assert.AreEqual(validationDevice, testDevice);
            Assert.AreEqual(validationTesterId, testTesterId);
            Assert.AreEqual(validationNetworkColorId, testNetworkColorId);
            Assert.AreEqual(validationProductionSiteId, testProductionSiteId);
        }

        [TestMethod]
        public void TestSerialNumberGenerated()
        {
            ZigbeeDeviceBase validationDevice = null;
            int? validationTesterId = null;
            int? validationSupervisorId = null;

            testFinalizeService.OnSerialNumberGenerated += delegate (ZigbeeDeviceBase device, int? testerId, int? supervisorId)
            {
                validationDevice = device;
                validationTesterId = testerId;
                validationSupervisorId = supervisorId;
            };

            var testDevice = new DemoZigbeeDeviceBase();
            var testTesterId = 9;
            var testSupervisorId = 99;

            testFinalizeService.SerialNumberGenerated(testDevice, testTesterId, testSupervisorId);

            Assert.AreEqual(validationDevice, testDevice);
            Assert.AreEqual(validationTesterId, testTesterId);
            Assert.AreEqual(validationSupervisorId, testSupervisorId);
        }

        [TestMethod]
        public void TestSerialNumberUpdated()
        {
            ZigbeeDeviceBase validationDevice = null;
            int? validationTesterId = null;

            testFinalizeService.OnSerialNumberUpdated += delegate (ZigbeeDeviceBase device, int? testerId)
            {
                validationDevice = device;
                validationTesterId = testerId;
            };

            var testDevice = new DemoZigbeeDeviceBase();
            var testTesterId = 9;

            testFinalizeService.SerialNumberUpdated(testDevice, testTesterId);

            Assert.AreEqual(validationDevice, testDevice);
            Assert.AreEqual(validationTesterId, testTesterId);
        }
    }
}
