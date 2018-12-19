using Centralite.Communications;
using Centralite.Database;
using Centralite.Ember.DataTypes;
using Centralite.Ember.Ezsp;
using System;

namespace Centralite.Common.Interfaces
{
    public interface IEzspService
    {
        LayerManager Manager { get; }
        EmberEui64 HostEui { get; }

        bool SetupManager(string serialPort);
        bool ConfigureEzsp();
        bool FormNetwork(NetworkColor networkColor);
        bool LeaveNetwork();
        bool SetPermitJoining(bool flag);
        bool SetSourceRoute(ushort destinationAddress, ushort[] relayList);

        void SendUnicast(EmberOutgoingMessageType type, ushort indexOrDestination, EmberApsFrame apsFrame, byte messageTag, byte[] messageContents);
        void SendBroadcast(ushort destination, EmberApsFrame apsFrame, byte radius, byte messageTag, byte[] messageContents);

        event Action<EzspMessageSentHandlerResponse> OnMessageSentResponse;
        event Action<EmberStatusResponse> OnScanErrorResponse;
        event Action<EzspScanCompleteHandlerResponse> OnScanCompleteResponse;
        event Action<EzspIncomingMessageHandlerResponse> OnIncomingMessageResponse;
        event Action<EmberStatusResponse> OnStackStatusResponse;
    }
}
