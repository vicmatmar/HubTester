using System;
using Centralite.Common.Interfaces;
using Centralite.Ember.DataTypes;
using Centralite.Utilities;
using Centralite.ZCL;
using CentraliteZigbeeLibrary.Centralite.ZCL;
using System.Collections.Generic;

namespace Centralite.Common.Utilities
{
    public static class HaMessageComposer
    {
        public static void SendModelStringRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.BasicCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);
            var b = new CommandBuffer();
            b.Add((byte)0x00); // Frame Control
            b.Add((byte)0x00); // Sequence Number
            b.Add(CommandIds.Global.ReadAttributes);
            b.Add(AttributeIds.BasicServer.ModelIdentifier);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, b.ToArray());
        }

        public static void SendFirmwareVersionRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);
            var b = new CommandBuffer();
            b.Add(ZclConstants.ZclFrameControlClusterServerToClientBit);
            b.Add((byte)0x00); // Sequence Number
            b.Add(CommandIds.Global.ReadAttributes);
            b.Add(AttributeIds.OtaServer.CurrentFileVersion);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, b.ToArray());
        }

        public static void WriteCieAddress(ushort address, EmberEui64 hostEui, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.IasZone, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Frame control
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.WriteAttributes); // Command
            buffer.Add(AttributeIds.IasZoneServer.CieAddress); // Attribute Identifier
            buffer.Add(ZclDataTypes.IeeeAddress); // Data Type
            buffer.Add(hostEui); // EUI to write

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SetLongPollRate(ushort address, uint quarterSeconds, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.PollControl, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlClusterSpecificBit); // Frame Control
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.PollControl.SetLongPollIntervalRate); // Command
            buffer.Add(quarterSeconds); // Attribute Identifier

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void RequestLongPollInvtervalMin(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.PollControl, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Control
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.PollControl.LongPollIntervalMin);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SetIasZoneMotionTimeout(ushort address, ushort timeout, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.IasZone, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlManufacturerSpecificBit); // Frame Control
            buffer.Add(ZclConstants.CentraliteManufacturerId); // Manufacturer Specific Code
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.WriteAttributes); // Command
            buffer.Add(AttributeIds.IasZoneServer.IasZoneMotionTimeout); // Attribute Identifier
            buffer.Add(ZclDataTypes.Int16u); // Data Type
            buffer.Add(timeout); // Timeout

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendRelayOnCommand(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.PearlThermostat, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlDisableDefaultResponseBit));
            buffer.Add(ZclConstants.CentraliteManufacturerProfileId);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.RelayOnOff.RelayCommand); // ZCL Command
            buffer.Add(CommandIds.RelayOnOff.RelayOn); // Relay-Command Command
            buffer.Add(CommandIds.RelayOnOff.AllRelaysIndex); // Relay Index

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendRelayOffCommand(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.PearlThermostat, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlDisableDefaultResponseBit));
            buffer.Add(ZclConstants.CentraliteManufacturerProfileId);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.RelayOnOff.RelayCommand); // ZCL Command
            buffer.Add(CommandIds.RelayOnOff.RelayOff); // Relay-Command Command
            buffer.Add(CommandIds.RelayOnOff.AllRelaysIndex); // Relay Index

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendIasEnrollResponse(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.IasZone, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlClusterSpecificBit);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.IasZoneClient.ZoneEnrollResponse);
            buffer.Add(ZCLStatus.ZclStatusSuccess);
            buffer.Add((byte)0x01); // Zone ID

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendIdentifyCommand(ushort address, ushort time, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.Identify, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlClusterSpecificBit);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.IdentifyClient.Identify);
            buffer.Add(time);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendTemperatureRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.TemperatureMeasurement, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Frame Control
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.TemperatureMeasurementServer.MeasuredValue);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendThermostatTemperatureRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.Thermostat, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Frame Control
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.Thermostat.LocalTemperature);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendDefaultCommand(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.BasicCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlClusterSpecificBit);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.ReadAttributes);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendHumidityRequest(ushort address, uint manufacturingCode, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.CentraliteHumidity, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlManufacturerSpecificBit); // Header
            buffer.Add(manufacturingCode); // MFG Code
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.ReadAttributes); // Command
            buffer.Add(AttributeIds.RelativeHumidityServer.MeasuredValue); // Attribute ID

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendOrientationRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.CentraliteAccelerometer, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlManufacturerSpecificBit);
            buffer.Add(ZclConstants.CentraliteManufacturerId);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.AccelerometerServer.AccelerometerOrientation);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendSelfTestCommand(ushort address, ushort cluster, ushort manufacturingCode, byte selfTestCommand, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, cluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlDisableDefaultResponseBit));
            buffer.Add(manufacturingCode);
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(selfTestCommand);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendProximityCalibrateRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.BasicCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlManufacturerSpecificBit));
            buffer.Add(ZclConstants.CentraliteManufacturerId);
            buffer.Add((byte)0x00);
            buffer.Add(CommandIds.Keypad.CheckCalibration);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendOnOffCommand(ushort address, byte command, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OnOffCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlClusterSpecificBit);
            buffer.Add((byte)0xb0);
            buffer.Add(command);
            buffer.AddArray(new byte[] { });

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendOnOffBroadcast(byte command, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OnOffCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add(ZclConstants.ZclFrameControlClusterSpecificBit);
            buffer.Add((byte)0x00);
            buffer.Add(command);

            ezspService.SendBroadcast(ZclConstants.BroadcastRoutersAndCoordinators, frame, 10, 0, buffer.ToArray());
        }

        public static void SendDimmerCommand(ushort address, ushort attribute, ushort transitionTime, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.LevelControl, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00);
            buffer.Add((byte)0x00);
            buffer.Add(CommandIds.LevelControl.Step);
            buffer.Add(attribute);
            buffer.Add(ZclDataTypes.Int16u);
            buffer.Add(transitionTime);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendDimmerBroadcast(ushort attribute, ushort transitionTime, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.LevelControl, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00);
            buffer.Add((byte)0x00);
            buffer.Add(CommandIds.LevelControl.Step);
            buffer.Add(attribute);
            buffer.Add(ZclDataTypes.Int16u);
            buffer.Add(transitionTime);

            ezspService.SendBroadcast(ZclConstants.BroadcastRoutersAndCoordinators, frame, 10, 0, buffer.ToArray());
        }

        public static void SendPowerMeasurementRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.ElectricalMeasurement, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00);
            buffer.Add((byte)0x00);
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.ElectricalMeasurement.ActivePower);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendReadMotorStatus(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OnOffCluster, 1, 3, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00);
            buffer.Add((byte)0x00);
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.OnOff.OnOffStatus);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendReadOccupancyStatus(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OccupancySensing, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00);
            buffer.Add((byte)0x00);
            buffer.Add(CommandIds.Global.ReadAttributes);
            buffer.Add(AttributeIds.OccupancySensingServer.MotionStatus);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendImageNotify(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterServerToClientBit | ZclConstants.ZclFrameControlClusterSpecificBit));
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.OtaUpgrade.ImageNotify);
            buffer.Add(ImageNotifyPayloadType.QueryJitter);
            buffer.Add((byte)100); // Query Jitter (0 - 100) Max Value should insure device responds

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendImageNotifyBroadcast(IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterServerToClientBit | ZclConstants.ZclFrameControlDisableDefaultResponseBit | ZclConstants.ZclFrameControlClusterSpecificBit));
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.OtaUpgrade.ImageNotify);
            buffer.Add(ImageNotifyPayloadType.QueryJitter);
            buffer.Add((byte)100); // Query Jitter (0 - 100) Max Value should insure device responds

            ezspService.SendBroadcast(ZclConstants.BroadcastAllDevices, frame, 10, 0, buffer.ToArray());
        }

        public static void SendQueryNextImageResponse(ushort address, byte status, IEzspService ezspService,
            ushort mfgCode = default(ushort), ushort imageType = default(ushort), uint fileVersion = default(uint), uint imageSize = default(uint))
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlDisableDefaultResponseBit | ZclConstants.ZclFrameControlClusterServerToClientBit | ZclConstants.ZclFrameControlClusterSpecificBit));
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.OtaUpgrade.QueryNextImageResponse);
            buffer.Add(status); // Status

            if (status == ZCLStatus.ZclStatusSuccess)
            {
                buffer.Add(mfgCode);
                buffer.Add(imageType);
                buffer.Add(fileVersion);
                buffer.Add(imageSize);
            }

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendImageBlockResponse(ushort address, byte status, IEzspService ezspService,
            ushort mfgCode = default(ushort), ushort imageType = default(ushort), uint fileVersion = default(uint),
            uint fileOffset = default(uint), byte dataSize = default(byte), byte[] imageData = null, ushort blockRequestDelay = default(ushort))
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlDisableDefaultResponseBit | ZclConstants.ZclFrameControlClusterServerToClientBit | ZclConstants.ZclFrameControlClusterSpecificBit));
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.OtaUpgrade.ImageBlockResponse);
            buffer.Add(status);

            if (status == ZCLStatus.ZclStatusSuccess)
            {
                buffer.Add(mfgCode);
                buffer.Add(imageType);
                buffer.Add(fileVersion);
                buffer.Add(fileOffset);
                buffer.Add(dataSize);
                buffer.AddArray(imageData);
            }
            else if (status == ZCLStatus.ZclStatusWaitForData)
            {
                // Next two commands tell the device to wait 1 second before requesting more data
                buffer.Add((uint)0x00); // Current Time (Base of wait calculation)
                buffer.Add((uint)0x01); // Request Time (Offset of wait calculation)

                buffer.Add(blockRequestDelay); // Ensure the BlockRequestDelay is set to 0
            }

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendUpgradeEndResponse(ushort address, ushort mfgCode, ushort imageType, uint fileVersion, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZclConstants.HomeAutomationProfileId, ClusterIds.OtaUpgradeCluster, 1, 1, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)(ZclConstants.ZclFrameControlClusterServerToClientBit | ZclConstants.ZclFrameControlClusterSpecificBit));
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(CommandIds.OtaUpgrade.UpgradeEndResponse);
            buffer.Add(mfgCode);
            buffer.Add(imageType);
            buffer.Add(fileVersion);
            buffer.Add((uint)0x00);
            buffer.Add((uint)0x05);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }
    }
}
