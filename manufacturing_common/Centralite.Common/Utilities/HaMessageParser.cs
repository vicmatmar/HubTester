using System.Text;
using Centralite.Ember.Ezsp;
using Centralite.Utilities;
using Centralite.ZCL;
using CentraliteZigbeeLibrary.Centralite.ZCL;
using Centralite.Ember.DataTypes;
using System.Linq;

namespace Centralite.Common.Utilities
{
    public static class HaMessageParser
    {
        private const byte COMMAND_ACROSS_ENTIRE_PROFILE_MASK = 0x03;
        private const int EUI_LENGTH = 8;


        public static bool TryParseModelString(EzspIncomingMessageHandlerResponse message, out string modelString)
        {
            bool result = false;
            modelString = null;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.BasicCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);

                    var control = buffer.ReadByte();
                    ushort? manufacturerCode = null;
                    if ((control & ZclConstants.ZclFrameControlManufacturerSpecificBit) == ZclConstants.ZclFrameControlManufacturerSpecificBit)
                    {
                        manufacturerCode = buffer.ReadUInt16();
                    }

                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (((control & COMMAND_ACROSS_ENTIRE_PROFILE_MASK) == 0x00) && (command == CommandIds.Global.ReadAttributesResponse))
                    {
                        var identifier = buffer.ReadUInt16();
                        var status = buffer.ReadByte();
                        if (status == ZCLStatus.ZclStatusSuccess)
                        {
                            if (identifier == AttributeIds.BasicServer.ModelIdentifier)
                            {
                                var dataType = buffer.ReadByte();
                                var length = buffer.ReadByte();

                                if (dataType == ZclDataTypes.CharacterString)
                                {
                                    modelString = Encoding.ASCII.GetString(buffer.ReadArray(length));
                                    result = true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseReadOnResponse(EzspIncomingMessageHandlerResponse message, out byte buttonId)
        {
            bool result = false;
            buttonId = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OnOffCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (command == CommandIds.OnOffClient.OnClusterCommand && (header & ZclConstants.ZclFrameControlClusterSpecificBit) != 0)
                    {
                        buttonId = message.ApsFrame.SourceEndpoint;
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseReadOffResponse(EzspIncomingMessageHandlerResponse message, out byte buttonId)
        {
            bool result = false;
            buttonId = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OnOffCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (command == CommandIds.OnOffClient.OffClusterCommand && (header & ZclConstants.ZclFrameControlClusterSpecificBit) != 0)
                    {
                        buttonId = message.ApsFrame.SourceEndpoint;
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }


        public static bool TryParseFirmwareVersion(EzspIncomingMessageHandlerResponse message, out uint firmwareVersion)
        {
            bool result = false;
            firmwareVersion = default(uint);

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OtaUpgradeCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Control
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();
                    var attributeId = buffer.ReadUInt16();
                    var status = buffer.ReadByte();
                    buffer.ReadByte(); // DataType

                    if (command == CommandIds.Global.ReadAttributesResponse && attributeId == AttributeIds.OtaServer.CurrentFileVersion && status == ZCLStatus.ZclStatusSuccess)
                    {
                        firmwareVersion = buffer.ReadUInt32();
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseLongPollIntervalMin(EzspIncomingMessageHandlerResponse message, out uint longPollIntervalMin)
        {
            bool result = false;
            longPollIntervalMin = uint.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.PollControl)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Control
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (command == CommandIds.Global.ReadAttributesResponse)
                    {
                        var attributeId = buffer.ReadUInt16();
                        if (attributeId == AttributeIds.PollControl.LongPollIntervalMin)
                        {
                            var status = buffer.ReadByte();
                            if (status == ZCLStatus.ZclStatusSuccess)
                            {
                                buffer.ReadByte(); // DataType

                                longPollIntervalMin = buffer.ReadUInt32();
                                result = true;
                            }
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseIasZoneStatusChange(EzspIncomingMessageHandlerResponse message, out ushort zoneStatus)
        {
            bool result = false;
            zoneStatus = ushort.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.IasZone)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if ((command == CommandIds.IasZoneServer.ZoneStatusChanged) && (header == (ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlClusterServerToClientBit)))
                    {
                        zoneStatus = buffer.ReadUInt16();
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseIasZoneEnrollRequest(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.IasZone)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Header
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (command == CommandIds.IasZoneServer.ZoneEnrollRequest)
                    {
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseTemperatureResponse(EzspIncomingMessageHandlerResponse message, out double temperature)
        {
            bool result = false;
            temperature = double.NaN;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.TemperatureMeasurement)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte(); // Header
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte(); // Command
                    if (command == CommandIds.Global.ReadAttributesResponse && (header & ZclConstants.ZclFrameControlClusterServerToClientBit) != 0)
                    {
                        var attributeId = buffer.ReadUInt16();
                        var status = buffer.ReadByte();

                        if (attributeId == AttributeIds.TemperatureMeasurementServer.MeasuredValue && status == ZCLStatus.ZclStatusSuccess)
                        {
                            var dataType = buffer.ReadByte(); // Data Type
                            if (dataType == ZclDataTypes.Int16)
                            {
                                var temp = buffer.ReadInt16();
                                temperature = ((temp / 100) * 1.8) + 32;
                                result = true;
                            }
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseThermostatTemperatureResponse(EzspIncomingMessageHandlerResponse message, out double temperature)
        {
            bool result = false;
            temperature = double.NaN;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.Thermostat)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte(); // Header
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte(); // Command
                    if (command == CommandIds.Global.ReadAttributesResponse && (header & ZclConstants.ZclFrameControlClusterServerToClientBit) != 0)
                    {
                        var attributeId = buffer.ReadUInt16();
                        var status = buffer.ReadByte();

                        if (attributeId == AttributeIds.TemperatureMeasurementServer.MeasuredValue && status == ZCLStatus.ZclStatusSuccess)
                        {
                            var dataType = buffer.ReadByte(); // Data Type
                            if (dataType == ZclDataTypes.Int16)
                            {
                                var temp = buffer.ReadInt16();
                                temperature = ((temp / 100) * 1.8) + 32;
                                result = true;
                            }
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseDeviceDefaultedResponse(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.BasicCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();
                    if (command == CommandIds.Global.DefaultResponse && (header & ZclConstants.ZclFrameControlClusterServerToClientBit) != 0)
                    {
                        var status = buffer.ReadByte();

                        result = status == ZCLStatus.ZclStatusSuccess;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseReadHumidityResponse(EzspIncomingMessageHandlerResponse message, uint expectedManufacturerCode, out double humidity)
        {
            bool result = false;
            humidity = double.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.CentraliteHumidity)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);

                    var header = buffer.ReadByte();
                    var manufacturerCode = buffer.ReadUInt16();
                    buffer.ReadByte();
                    var command = buffer.ReadByte();

                    if (manufacturerCode == expectedManufacturerCode && command == CommandIds.Global.ReadAttributesResponse && (header & ZclConstants.ZclFrameControlClusterServerToClientBit) != 0)
                    {
                        var attributeId = buffer.ReadUInt16();
                        var status = buffer.ReadByte();

                        if (attributeId == AttributeIds.TemperatureMeasurementServer.MeasuredValue && status == ZCLStatus.ZclStatusSuccess)
                        {
                            buffer.ReadByte(); // Data Type
                            double humidityValue = buffer.ReadUInt16();

                            humidity = humidityValue / 100;
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseReadOrientationResponse(EzspIncomingMessageHandlerResponse message, out byte orienationResult)
        {
            bool result = false;
            orienationResult = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.CentraliteAccelerometer)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);

                    var header = buffer.ReadByte();
                    var mfgCode = buffer.ReadUInt16();

                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (mfgCode == ZclConstants.CentraliteManufacturerId &&
                        command == CommandIds.Global.ReadAttributesResponse &&
                        (header & ZclConstants.ZclFrameControlClusterServerToClientBit) != 0)
                    {
                        var attributeId = buffer.ReadUInt16();
                        var status = buffer.ReadByte();

                        if (attributeId == AttributeIds.AccelerometerServer.AccelerometerOrientation && status == ZCLStatus.ZclStatusSuccess)
                        {
                            var dataType = buffer.ReadByte();

                            if (dataType == ZclDataTypes.Enumeration8Bit)
                            {
                                orienationResult = buffer.ReadByte();
                                result = true;
                            }
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseButtonPressedResponse(EzspIncomingMessageHandlerResponse message, out byte buttonId)
        {
            bool result = false;
            buttonId = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.BasicCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    if ((header & ZclConstants.ZclFrameControlManufacturerSpecificBit) != 0)
                    {
                        var mfgCode = buffer.ReadUInt16();
                        buffer.ReadByte(); // Sequence
                        var command = buffer.ReadByte();

                        if (mfgCode == ZclConstants.CentraliteManufacturerId &&
                            command == CommandIds.EnterSelfTest.ButtonPressed &&
                            header == (ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlClusterServerToClientBit))
                        {
                            buttonId = buffer.ReadByte();
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseProximityCalibrationResponse(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.BasicCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    var mfgCode = buffer.ReadUInt16();
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (mfgCode == ZclConstants.CentraliteManufacturerId &&
                        command == CommandIds.Keypad.CheckCalibrationResponse &&
                        header == (ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlClusterServerToClientBit))
                    {
                        var calibrationValue = buffer.ReadUInt16();

                        if (calibrationValue > ushort.MinValue && calibrationValue < ushort.MaxValue)
                        {
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseProximityDetectedResponse(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.BasicCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();
                    var mfgCode = buffer.ReadUInt16();
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();

                    if (mfgCode == ZclConstants.CentraliteManufacturerId &&
                        command == CommandIds.Keypad.ProximityDetected &&
                        header == (ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlClusterSpecificBit | ZclConstants.ZclFrameControlClusterServerToClientBit))
                    {
                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParsePowerMeasurementResponse(EzspIncomingMessageHandlerResponse message, out double power)
        {
            bool result = false;
            power = double.NaN;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.ElectricalMeasurement)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Header
                    buffer.ReadByte(); // Sequence
                    var command = buffer.ReadByte();
                    if (command == CommandIds.Global.ReadAttributesResponse)
                    {
                        var attribute = buffer.ReadUInt16();
                        var status = buffer.ReadByte();
                        if (status == ZCLStatus.ZclStatusSuccess)
                        {
                            buffer.ReadByte(); // DataType
                            var tempPower = buffer.ReadInt16();
                            power = (double)tempPower / 10.0;
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseOnOffDefaultResponse(EzspIncomingMessageHandlerResponse message, out byte payloadCommand)
        {
            bool result = false;
            payloadCommand = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OnOffCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte();
                    buffer.ReadByte();
                    var command = buffer.ReadByte();

                    if (command == CommandIds.Global.DefaultResponse)
                    {
                        var payload = buffer.ReadByte();
                        var status = buffer.ReadByte();

                        if (status == ZCLStatus.ZclStatusSuccess)
                        {
                            payloadCommand = payload;
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseOnOffStatusResponse(EzspIncomingMessageHandlerResponse message, out byte onoffStatus)
        {
            bool result = false;
            onoffStatus = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OnOffCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte();
                    buffer.ReadByte();
                    var command = buffer.ReadByte();

                    if (command == CommandIds.Global.ReadAttributesResponse)
                    {
                        var attribute = buffer.ReadUInt16();
                        var status = buffer.ReadByte();

                        if (attribute == AttributeIds.OnOff.OnOffStatus && status == ZCLStatus.ZclStatusSuccess)
                        {
                            buffer.ReadByte(); // Datatype
                            onoffStatus = buffer.ReadByte();
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseOccupancyStatusResponse(EzspIncomingMessageHandlerResponse message, out byte occupancyStatus)
        {
            bool result = false;
            occupancyStatus = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OccupancySensing)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte();
                    buffer.ReadByte();
                    var command = buffer.ReadByte();

                    if (command == CommandIds.Global.ReadAttributesResponse)
                    {
                        var attribute = buffer.ReadUInt16();
                        var status = buffer.ReadByte();

                        if (attribute == AttributeIds.OccupancySensingServer.MotionStatus && status == ZCLStatus.ZclStatusSuccess)
                        {
                            buffer.ReadByte(); // Datatype
                            occupancyStatus = buffer.ReadByte();
                            result = true;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseSelfTestDefaultResponse(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.BasicCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var header = buffer.ReadByte();

                    if (header == (ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlClusterServerToClientBit) ||
                        header == (ZclConstants.ZclFrameControlManufacturerSpecificBit | ZclConstants.ZclFrameControlClusterServerToClientBit | ZclConstants.ZclFrameControlClusterSpecificBit))
                    {
                        var mfgCode = buffer.ReadUInt16();
                        buffer.ReadByte();
                        var command = buffer.ReadByte();
                        if (mfgCode == ZclConstants.CentraliteManufacturerId && command == CommandIds.Global.DefaultResponse)
                        {
                            var status = buffer.ReadByte();

                            return status == ZCLStatus.ZclStatusSuccess;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseQueryNextImageRequest(EzspIncomingMessageHandlerResponse message, out ushort mfgCode, out ushort imageType, out uint fileVersion)
        {
            bool result = false;
            mfgCode = ushort.MaxValue;
            imageType = ushort.MaxValue;
            fileVersion = uint.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OtaUpgradeCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var control = buffer.ReadByte();

                    buffer.ReadByte(); // Sequence Number
                    var command = buffer.ReadByte();

                    if (command == CommandIds.OtaUpgrade.QueryNextImageRequest)
                    {
                        var fieldControl = buffer.ReadByte();
                        mfgCode = buffer.ReadUInt16();
                        imageType = buffer.ReadUInt16();
                        fileVersion = buffer.ReadUInt32();

                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseImagePageRequest(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OtaUpgradeCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var control = buffer.ReadByte();
                    buffer.ReadByte();
                    var command = buffer.ReadByte();

                    if (command == CommandIds.OtaUpgrade.ImagePageRequest)
                    {
                        var fieldControl = buffer.ReadByte();
                        var mfgCode = buffer.ReadUInt16();
                        var imageType = buffer.ReadUInt16();
                        var fileVersion = buffer.ReadUInt16();
                        var fileOffset = buffer.ReadUInt16();
                        var maxDataSize = buffer.ReadByte();
                        var pageSize = buffer.ReadUInt16();
                        var responseSpacing = buffer.ReadUInt16();

                        if ((fieldControl & 0x01) != 0)
                        {
                            var ieeeAddress = buffer.ReadArray(EUI_LENGTH).Reverse().ToArray();
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseImageBlockRequestCommand(EzspIncomingMessageHandlerResponse message,
            out ushort mfgCode, out ushort imageType, out uint fileVersion, out uint fileOffset, out byte maxDataSize, out ushort blockRequestDelay)
        {
            bool result = false;
            mfgCode = ushort.MaxValue;
            imageType = ushort.MaxValue;
            fileVersion = uint.MaxValue;
            fileOffset = uint.MaxValue;
            maxDataSize = byte.MaxValue;
            blockRequestDelay = ushort.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OtaUpgradeCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var control = buffer.ReadByte();

                    buffer.ReadByte(); // Sequence Number
                    var command = buffer.ReadByte();

                    if (command == CommandIds.OtaUpgrade.ImageBlockRequest)
                    {
                        var fieldControl = buffer.ReadByte();
                        mfgCode = buffer.ReadUInt16();
                        imageType = buffer.ReadUInt16();
                        fileVersion = buffer.ReadUInt32();
                        fileOffset = buffer.ReadUInt32();
                        maxDataSize = buffer.ReadByte();

                        EmberEui64 eui;
                        if ((fieldControl & ImageBlockRequestFieldControl.IEEEAddressPresent) != 0)
                        {
                            eui = new EmberEui64(buffer.ReadArray(EUI_LENGTH).Reverse().ToArray());
                        }

                        if ((fieldControl & ImageBlockRequestFieldControl.BlockRequestDelayPresent) != 0)
                        {
                            blockRequestDelay = buffer.ReadUInt16();
                        }

                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseUpgradeEndRequestCommand(EzspIncomingMessageHandlerResponse message, out byte status)
        {
            bool result = false;
            status = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZclConstants.HomeAutomationProfileId && message.ApsFrame.ClusterId == ClusterIds.OtaUpgradeCluster)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    var control = buffer.ReadByte();

                    buffer.ReadByte(); // Sequence Number
                    var command = buffer.ReadByte();

                    if (command == CommandIds.OtaUpgrade.UpgradeEndRequest)
                    {
                        status = buffer.ReadByte();
                        var mfgCode = buffer.ReadUInt16();
                        var imageType = buffer.ReadUInt16();
                        var fileVersion = buffer.ReadUInt32();

                        result = true;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
