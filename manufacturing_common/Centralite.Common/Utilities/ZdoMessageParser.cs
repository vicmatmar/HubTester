using Centralite.Ember.DataTypes;
using Centralite.Ember.Ezsp;
using Centralite.Utilities;
using Centralite.ZCL;
using Centralite.ZDO;
using System.Linq;

namespace Centralite.Common.Utilities
{
    public static class ZdoMessageParser
    {
        private const int EUI_LENGTH = 8;

        public static bool TryParseDeviceAnnounce(EzspIncomingMessageHandlerResponse message, out EmberEui64 eui)
        {
            bool result = false;
            eui = null;

            if (message.ApsFrame.ProfileId == ZdoConstants.ZdoProfileId && message.ApsFrame.ClusterId == ZdoClusterIds.DeviceAnnounce)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Read Sequence Number
                    buffer.ReadUInt16(); // Read Address

                    eui = new EmberEui64(buffer.ReadArray(EUI_LENGTH).Reverse().ToArray());
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseIEEEAddressResponse(EzspIncomingMessageHandlerResponse message, out EmberEui64 eui)
        {
            bool result = false;
            eui = null;

            if (message.ApsFrame.ProfileId == ZdoConstants.ZdoProfileId && message.ApsFrame.ClusterId == ZdoClusterIds.IEEEAddressResponse)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Read Sequence Number
                    buffer.ReadByte(); // Read Status

                    eui = new EmberEui64(buffer.ReadArray(EUI_LENGTH).Reverse().ToArray());
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseLeaveResponse(EzspIncomingMessageHandlerResponse message)
        {
            bool result = false;

            if (message.ApsFrame.ProfileId == ZdoConstants.ZdoProfileId && message.ApsFrame.ClusterId == ZdoClusterIds.ManagementLeaveResponse && message.ApsFrame.DestinationEndpoint == 0 && message.ApsFrame.SourceEndpoint == 0)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Sequence Number

                    var status = buffer.ReadByte(); // Status
                    result = status == ZCLStatus.ZclStatusSuccess;
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static bool TryParseActiveEndpointResponse(EzspIncomingMessageHandlerResponse message, out ushort networkAddress, out byte endpointCount)
        {
            bool result = false;
            networkAddress = ushort.MaxValue;
            endpointCount = byte.MaxValue;

            if (message.ApsFrame.ProfileId == ZdoConstants.ZdoProfileId && message.ApsFrame.ClusterId == ZdoClusterIds.ActiveEndpointResponse)
            {
                try
                {
                    var buffer = new CommandBuffer(message.MessageContents);
                    buffer.ReadByte(); // Sequence
                    var status = buffer.ReadByte();

                    if (status == ZCLStatus.ZclStatusSuccess)
                    {
                        networkAddress = buffer.ReadUInt16();
                        endpointCount = buffer.ReadByte();
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
