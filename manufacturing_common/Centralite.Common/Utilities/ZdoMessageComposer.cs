using System;
using Centralite.Common.Interfaces;
using Centralite.Ember.DataTypes;
using Centralite.Utilities;
using Centralite.ZDO;
using Centralite.ZCL;

namespace Centralite.Common.Utilities
{
    public static class ZdoMessageComposer
    {
        public static void SendIEEEAddressRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.IEEEAddressRequest, 0, 0, EmberApsOption.EMBER_APS_OPTION_NONE, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(address); // Network Address
            buffer.Add((byte)0x00); // Request Type 0x00 = Single Device Response 0x01 = Extended Response
            buffer.Add((byte)0x00); // Start Index (Used if Extended Response)

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendLeaveRequest(ushort address, bool sleepy, byte tag, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.ManagementLeaveRequest, 0, 0, EmberApsOption.EMBER_APS_OPTION_NONE, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00);
            buffer.Add(0);
            buffer.Add(0);
            buffer.Add((byte)0x00);

            var emptySourceRoute = new ushort[0];

            if (!sleepy)
            {
                ezspService.SetSourceRoute(address, emptySourceRoute);
            }

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, tag, buffer.ToArray());
        }

        public static void SendOnOffClusterBindRequest(ushort address, EmberEui64 eui, byte endpointCount, IEzspService ezspService)
        {
            for (byte i = 1; i <= endpointCount; i++)
            {
                var frame = new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.BindRequest, i, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

                var buffer = new CommandBuffer();
                buffer.Add((byte)0x00); // Sequence
                buffer.Add(eui); // Device EUI
                buffer.Add(i); // Endpoint for device
                buffer.Add(ZCL.ClusterIds.OnOffCluster);
                buffer.Add((byte)0x03); // Mode
                buffer.Add(ezspService.HostEui); // Host EUI
                buffer.Add((byte)0x01); // Endpoint for host

                ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
            }
        }

        public static void SendActiveEndpointRequest(ushort address, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.ActiveEndpointRequest, 0, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(address);

            ezspService.SendUnicast(EmberOutgoingMessageType.EMBER_OUTGOING_DIRECT, address, frame, 0, buffer.ToArray());
        }

        public static void SendPermitJoiningBroadcast(byte duration, IEzspService ezspService)
        {
            var frame = new EmberApsFrame(ZdoConstants.ZdoProfileId, ZdoClusterIds.PermitJoiningRequest, 0, 0, EmberApsOption.EMBER_APS_OPTION_STANDARD, 0, 0);

            var buffer = new CommandBuffer();
            buffer.Add((byte)0x00); // Sequence
            buffer.Add(duration);
            buffer.Add((byte)0x00); // Trust Center Significance

            ezspService.SendBroadcast(ZclConstants.BroadcastRoutersAndCoordinators, frame, 10, 0, buffer.ToArray());
        }
    }
}
