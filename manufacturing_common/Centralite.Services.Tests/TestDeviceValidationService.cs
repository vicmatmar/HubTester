using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using Centralite.Database;
using Centralite.Ember.DataTypes;
using Centralite.Ember.Ezsp;
using Centralite.Tests;
using Centralite.Utilities;
using Centralite.ZCL;
using Centralite.ZDO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Centralite.Services.Tests
{
    [TestClass]
    public class TestDeviceValidationService
    {
        private readonly Mock<IEzspService> mockEzspService = new Mock<IEzspService>();
        private readonly Mock<IDataContextFactory> mockDataContextFactory = new Mock<IDataContextFactory>();
        private readonly Mock<IErrorProducerService> mockErrorProducerService = new Mock<IErrorProducerService>();

        private DeviceValidationService deviceValidationService;

        private EzspIncomingMessageHandlerResponse deviceAnnounceMessage;
        private EzspIncomingMessageHandlerResponse ieeeAddressMessage;
        private EzspIncomingMessageHandlerResponse modelStringMessage;
        private EzspIncomingMessageHandlerResponse firmwareVersionMessage;

        private const string TEST_CHIPTYPE_NAME = "Not EM250";
        private const string TEST_MODEL_STRING = "TestModelString";
        private const string TEST_SKU = "TestkSKU";
        private const string TEST_EUI = "FFFFFFFFFFFFFFFF";
        private const uint TEST_FIRMWARE_VERSION = 1111111111;
        private const ushort TEST_ADDRESS = 0xFFFF;

        [TestInitialize]
        public void SetupTests()
        {
            Mock<IManufacturingStoreRepository> mockManufacturingStoreRepository = new Mock<IManufacturingStoreRepository>();

            var product = new Product()
            {
                Board = new Board()
                {
                    ChipType = new ChipType()
                    {
                        Name = TEST_CHIPTYPE_NAME
                    }
                },
                ModelString = TEST_MODEL_STRING,
                SKU = TEST_SKU
            };

            var targetDevices = new List<TargetDevice>();
            targetDevices.Add(new TargetDevice()
            {
                TestSession = new TestSession()
                {
                    Product = product
                }
            });

            var mockTargetDevices = MockHelper.MockDbSet(targetDevices);

            var euiLists = new List<EuiList>();

            euiLists.Add(new EuiList()
            {
                Id = 0,
                EUI = TEST_EUI,
                TargetDevices = targetDevices
            });

            var mockEuiLists = MockHelper.MockDbSet(euiLists);

            mockManufacturingStoreRepository.Setup(x => x.EuiLists).Returns(mockEuiLists);
            mockManufacturingStoreRepository.Setup(x => x.TargetDevices).Returns(mockTargetDevices);

            deviceValidationService = new DeviceValidationService(mockEzspService.Object, mockDataContextFactory.Object, mockErrorProducerService.Object);

            mockEzspService.SetupAllProperties();
            mockDataContextFactory.SetupAllProperties();
            mockDataContextFactory.Setup(x => x.CreateManufacturingStoreRepository()).Returns(mockManufacturingStoreRepository.Object);
            mockErrorProducerService.SetupAllProperties();

            deviceValidationService.ValidProduct = product;

            var deviceAnnounceBuffer = new CommandBuffer();
            deviceAnnounceBuffer.Add((byte)0);
            deviceAnnounceBuffer.Add(new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.DeviceAnnounce, 0, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0));
            deviceAnnounceBuffer.Add((byte)0);
            deviceAnnounceBuffer.Add((sbyte)0);
            deviceAnnounceBuffer.Add((uint)0xFFFF);
            deviceAnnounceBuffer.Add((byte)11);
            deviceAnnounceBuffer.AddArray(new byte[] { 0, 0, 0, 255, 255, 255, 255, 255, 255, 255, 255 });
            deviceAnnounceMessage = new EzspIncomingMessageHandlerResponse(deviceAnnounceBuffer);

            var ieeeAddressBuffer = new CommandBuffer();
            ieeeAddressBuffer.Add((byte)0);
            ieeeAddressBuffer.Add(new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.IEEEAddressResponse, 0, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0));
            ieeeAddressBuffer.Add((byte)0);
            ieeeAddressBuffer.Add((sbyte)0);
            ieeeAddressBuffer.Add((uint)0xFFFF);
            ieeeAddressBuffer.Add((byte)10);
            ieeeAddressBuffer.AddArray(new byte[] { 0, 0, 255, 255, 255, 255, 255, 255, 255, 255 });
            ieeeAddressMessage = new EzspIncomingMessageHandlerResponse(ieeeAddressBuffer);

            var modelStringArray = Encoding.ASCII.GetBytes(TEST_MODEL_STRING);
            var modelStringBuffer = new CommandBuffer();
            modelStringBuffer.Add((byte)0);
            modelStringBuffer.Add(new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.BasicCluster, 0, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0));
            modelStringBuffer.Add((byte)0);
            modelStringBuffer.Add((sbyte)0);
            modelStringBuffer.Add((uint)0xFFFF);
            modelStringBuffer.Add(((byte)(modelStringArray.Count() + 10)));
            modelStringBuffer.Add(ZclConstants.ZclFrameControlManufacturerSpecificBit);
            modelStringBuffer.Add(ZclConstants.CentraliteManufacturerId);
            modelStringBuffer.Add((byte)0);
            modelStringBuffer.Add(CommandIds.Global.ReadAttributesResponse);
            modelStringBuffer.Add(AttributeIds.BasicServer.ModelIdentifier);
            modelStringBuffer.Add(ZCLStatus.ZclStatusSuccess);
            modelStringBuffer.Add(ZclDataTypes.CharacterString);
            modelStringBuffer.Add(((byte)modelStringArray.Count()));
            modelStringBuffer.AddArray(modelStringArray);
            modelStringMessage = new EzspIncomingMessageHandlerResponse(modelStringBuffer);

            var firmwareVersionBuffer = new CommandBuffer();
            firmwareVersionBuffer.Add((byte)0);
            firmwareVersionBuffer.Add(new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 0, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0));
            firmwareVersionBuffer.Add((byte)0);
            firmwareVersionBuffer.Add((sbyte)0);
            firmwareVersionBuffer.Add((uint)0xFFFF);
            firmwareVersionBuffer.Add((byte)11);
            firmwareVersionBuffer.Add((byte)0);
            firmwareVersionBuffer.Add((byte)0);
            firmwareVersionBuffer.Add(CommandIds.Global.ReadAttributesResponse);
            firmwareVersionBuffer.Add(AttributeIds.OtaServer.CurrentFileVersion);
            firmwareVersionBuffer.Add(ZCLStatus.ZclStatusSuccess);
            firmwareVersionBuffer.Add((byte)0);
            firmwareVersionBuffer.Add(TEST_FIRMWARE_VERSION);
            firmwareVersionMessage = new EzspIncomingMessageHandlerResponse(firmwareVersionBuffer);
        }

        [TestMethod]
        public void TestValidateDevice_DeviceAnnounceMessage()
        {
            bool deviceAddedEventCalled = false;
            deviceValidationService.OnAddDevice += delegate (ZigbeeDeviceBase testDevice)
            {
                deviceAddedEventCalled = true;
            };

            var device = new DemoZigbeeDeviceBase() { Address = TEST_ADDRESS };

            deviceValidationService.ValidateDevice(device, deviceAnnounceMessage);

            Assert.IsTrue(deviceAddedEventCalled);
            Assert.AreEqual(device.Eui.ToString(), TEST_EUI);
            mockEzspService.Verify(x => x.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, TEST_ADDRESS, It.IsAny<EmberApsFrame>(), 0, It.IsAny<byte[]>()));
        }

        [TestMethod]
        public void TestValidateDevice_IEEEAddressMessage()
        {
            bool deviceAddedEventCalled = false;
            deviceValidationService.OnAddDevice += delegate (ZigbeeDeviceBase testDevice)
            {
                deviceAddedEventCalled = true;
            };

            var device = new DemoZigbeeDeviceBase() { Address = TEST_ADDRESS };

            deviceValidationService.ValidateDevice(device, ieeeAddressMessage);

            Assert.IsTrue(deviceAddedEventCalled);
            Assert.AreEqual(device.Eui.ToString(), TEST_EUI);
            mockEzspService.Verify(x => x.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, TEST_ADDRESS, It.IsAny<EmberApsFrame>(), 0, It.IsAny<byte[]>()));
        }

        [TestMethod]
        public void TestValidateDevice_ModelStringMessage_Success()
        {
            var device = new DemoZigbeeDeviceBase()
            {
                Address = TEST_ADDRESS,
                Eui = new EmberEui64(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 })
            };

            deviceValidationService.ValidateDevice(device, modelStringMessage);

            Assert.AreEqual(device.ModelString, TEST_MODEL_STRING);
            mockEzspService.Verify(x => x.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, TEST_ADDRESS, It.IsAny<EmberApsFrame>(), 0, It.IsAny<byte[]>()));
        }

        [TestMethod]
        public void TestValidateDevice_ModelStringMessage_Failure()
        {
            bool deviceRemovedEventCalled = false;
            deviceValidationService.OnRemoveDevice += delegate (ZigbeeDeviceBase testDevice)
            {
                deviceRemovedEventCalled = true;
            };

            var device = new DemoZigbeeDeviceBase()
            {
                Address = TEST_ADDRESS,
                Eui = new EmberEui64(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 })
            };

            deviceValidationService.ValidProduct = new Product() { ModelString = "Not TestModelString" };
            deviceValidationService.ValidateDevice(device, modelStringMessage);

            Assert.IsTrue(deviceRemovedEventCalled);
            mockEzspService.Verify(x => x.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, TEST_ADDRESS, It.IsAny<EmberApsFrame>(), 0, It.IsAny<byte[]>()));
        }

        [TestMethod]
        public void TestValidateDevice_FirmwareVersionMessage()
        {
            var device = new DemoZigbeeDeviceBase()
            {
                Address = TEST_ADDRESS,
                Eui = new EmberEui64(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }),
                ModelString = TEST_MODEL_STRING
            };

            deviceValidationService.ValidateDevice(device, firmwareVersionMessage);

            Assert.IsTrue(device.Validated);
            Assert.AreEqual(device.LongFirmwareVersion, TEST_FIRMWARE_VERSION);
        }
    }
}
