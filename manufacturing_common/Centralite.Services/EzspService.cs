using System;
using System.Collections.Generic;
using Centralite.Communications;
using System.ComponentModel.Composition;
using Centralite.Communications.Protocol;
using Centralite.Communications.Hardware;
using Centralite.Ember.Ezsp;
using Centralite.Ember.DataTypes;
using Centralite.Database;
using Centralite.Common.Interfaces;
using Centralite.ZCL;
using Centralite.Common.Models;
using Centralite.Common.Utilities;

namespace Centralite.Services
{
    [Export(typeof(IEzspService))]
    public class EzspService : IEzspService
    {
        private IErrorProducerService errorProducerService;

        private const int BaudRate = 115200;
        private const sbyte DefaultTxPowerLveldBm = 3;

        private EmberEzspLayer Ezsp;

        public event Action<EzspMessageSentHandlerResponse> OnMessageSentResponse;
        public event Action<EmberStatusResponse> OnScanErrorResponse;
        public event Action<EzspScanCompleteHandlerResponse> OnScanCompleteResponse;
        public event Action<EzspIncomingMessageHandlerResponse> OnIncomingMessageResponse;
        public event Action<EmberStatusResponse> OnStackStatusResponse;

        public LayerManager Manager { get; private set; }

        public EmberEui64 HostEui
        {
            get
            {
                return this.Ezsp.GetEui64();
            }
        }

        [ImportingConstructor]
        public EzspService(IErrorProducerService errorProducerService)
        {
            this.errorProducerService = errorProducerService;
        }

        public bool SetupManager(string serialPort)
        {
            if (String.IsNullOrEmpty(serialPort)) return false;

            Manager = new LayerManager();

            Ezsp = new EmberEzspLayer(Manager);
            Ezsp.StackStatusHandler += Ezsp_StackStatusHandler;
            Ezsp.IncomingMessageHandler += Ezsp_IncomingMessageHandler;
            Ezsp.ScanCompleteHandler += Ezsp_ScanCompleteHandler;
            Ezsp.ScanErrorHandler += Ezsp_ScanErrorHandler;
            Ezsp.MessageSentHandler += Ezsp_MessageSentHandler;

            var ash = new Centralite.Communications.Transport.EmberASHLayer(Manager);
            var hardware = new SerialLayer(Manager);
            hardware.SetConnection(serialPort, BaudRate);

            Manager.AddLayer(hardware);
            Manager.AddLayer(ash);
            Manager.AddLayer(Ezsp);

            return true;
        }

        public bool ConfigureEzsp()
        {
            bool result = true;

            var policyResponse = Ezsp.SetPolicy(EzspPolicyId.EZSP_UNICAST_REPLIES_POLICY, EzspDecisionId.EZSP_HOST_WILL_NOT_SUPPLY_REPLY);

            result &= (policyResponse?.Status == EmberStatus.EMBER_SUCCESS);

            var configs = new List<EzspSetConfigurationValue>();
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_STACK_PROFILE, 2));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_PAN_ID_CONFLICT_REPORT_THRESHOLD, 10));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_SOURCE_ROUTE_TABLE_SIZE, 10));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_TX_POWER_MODE, 0));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_MAX_END_DEVICE_CHILDREN, 20));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_INDIRECT_TRANSMISSION_TIMEOUT, 8 * 1000));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_ADDRESS_TABLE_SIZE, 20));
            configs.Add(new EzspSetConfigurationValue(EzspConfigId.EZSP_CONFIG_APS_UNICAST_MESSAGE_COUNT, 10));

            foreach (var config in configs)
            {
                var res = Ezsp.SetConfigurationValue(config.ConfigId, config.Value);

                result &= (res?.Status == EzspStatus.EZSP_SUCCESS);
            }

            var endpointResponse = Ezsp.AddEndpoint(1, ZclConstants.HomeAutomationProfileId, 1, 0, new ushort[] { ZCL.ClusterIds.OtaUpgradeCluster }, new ushort[] { ZCL.ClusterIds.IasZone }) ?? new EmberStatusResponse(EmberStatus.EMBER_ERR_FATAL);
            result &= (endpointResponse?.Status == EmberStatus.EMBER_SUCCESS);

            if (!result)
            {
                errorProducerService.AddMessage(new ErrorMessage("Unable to configure USB Stick", ErrorType.Error));
            }

            return result;
        }

        public bool FormNetwork(NetworkColor networkColor)
        {
            bool result;

            if (networkColor == null)
            {
                result = false;
            }
            else
            {
                var securityStateResponse = ConfigureSecurityState();

                if (securityStateResponse)
                {
                    var res = Ezsp.FormNetwork(new EmberNetworkParameters(new EmberExtendedPanId(networkColor.ExtendedPan), (ushort)(networkColor.Pan), DefaultTxPowerLveldBm, (byte)networkColor.Channel));

                    result = (res?.Status == EmberStatus.EMBER_SUCCESS);
                }
                else
                {
                    result = false;
                }
            }

            if (!result)
            {
                errorProducerService.AddMessage(new ErrorMessage("Unable to form network", ErrorType.Error));
            }

            return result;
        }

        public bool LeaveNetwork()
        {
            var res = Ezsp.LeaveNetwork();

            if (res?.Status != EmberStatus.EMBER_SUCCESS)
            {
                errorProducerService.AddMessage(new ErrorMessage("Unable to leave network", ErrorType.Exception));
            }

            return (res?.Status == EmberStatus.EMBER_SUCCESS);
        }

        public bool SetPermitJoining(bool flag)
        {
            EmberStatusResponse response;

            if (flag)
            {
                response = Ezsp.PermitJoining(byte.MaxValue);
            }
            else
            {
                response = Ezsp.PermitJoining(0);
            }

            if (response?.Status != EmberStatus.EMBER_SUCCESS)
            {
                errorProducerService.AddMessage(new ErrorMessage("Unable to set permit joining", ErrorType.Error));
            }

            return (response?.Status == EmberStatus.EMBER_SUCCESS);
        }

        public bool SetSourceRoute(ushort destinationAddress, ushort[] relayList)
        {
            var response = Ezsp.SetSourceRoute(destinationAddress, relayList);

            if (response?.Status != EmberStatus.EMBER_SUCCESS)
            {
                errorProducerService.AddMessage(new ErrorMessage("Unable to set source route", ErrorType.Exception));
            }

            return (response?.Status == EmberStatus.EMBER_SUCCESS);
        }

        public void SendUnicast(EmberOutgoingMessageType type, ushort indexOrDestination, EmberApsFrame apsFrame, byte messageTag, byte[] messageContents)
        {
            BackgroundQueue.QueueTask(() => Ezsp.SendUnicast(type, indexOrDestination, apsFrame, messageTag, messageContents));
        }

        public void SendBroadcast(ushort destination, EmberApsFrame apsFrame, byte radius, byte messageTag, byte[] messageContents)
        {
            BackgroundQueue.QueueTask(() => Ezsp.SendBroadcast(destination, apsFrame, radius, messageTag, messageContents));
        }

        private bool ConfigureSecurityState()
        {
            Random r = new Random();
            EmberInitialSecurityBitmask bitmask = (EmberInitialSecurityBitmask.EMBER_STANDARD_SECURITY_MODE
                                                            | EmberInitialSecurityBitmask.EMBER_HAVE_PRECONFIGURED_KEY
                                                            | EmberInitialSecurityBitmask.EMBER_GLOBAL_LINK_KEY
                                                            | EmberInitialSecurityBitmask.EMBER_REQUIRE_ENCRYPTED_KEY
                                                            | EmberInitialSecurityBitmask.EMBER_HAVE_NETWORK_KEY);

            EmberKeyData preconfigKey = new Centralite.Ember.DataTypes.EmberKeyData(new byte[] { 0x5A, 0x69, 0x67, 0x42, 0x65, 0x65, 0x41, 0x6C, 0x6C, 0x69, 0x61, 0x6E, 0x63, 0x65, 0x30, 0x39 });
            byte[] key = new byte[16];
            r.NextBytes(key);
            EmberKeyData networkKey = new EmberKeyData(key);

            var res = Ezsp.SetInitialSecurityState(new EmberInitialSecurityState(bitmask, preconfigKey, networkKey, 0)) ?? new EmberStatusResponse(EmberStatus.EMBER_ERR_FATAL);

            if (res?.Status != EmberStatus.EMBER_SUCCESS)
            {
                errorProducerService.AddMessage(new ErrorMessage("Unable to configure security state of USB stick", ErrorType.Exception));
            }

            return (res?.Status == EmberStatus.EMBER_SUCCESS);
        }

        #region EZSP Events
        private void Ezsp_MessageSentHandler(object sender, EzspMessageSentHandlerResponse response)
        {
            OnMessageSentResponse?.Invoke(response);
        }

        private void Ezsp_ScanErrorHandler(object sender, EmberStatusResponse response)
        {
            OnScanErrorResponse?.Invoke(response);
        }

        private void Ezsp_ScanCompleteHandler(object sender, EzspScanCompleteHandlerResponse response)
        {
            OnScanCompleteResponse?.Invoke(response);
        }

        private void Ezsp_IncomingMessageHandler(object sender, EzspIncomingMessageHandlerResponse response)
        {
            OnIncomingMessageResponse?.Invoke(response);
        }

        private void Ezsp_StackStatusHandler(object sender, EmberStatusResponse response)
        {
            OnStackStatusResponse?.Invoke(response);
        }
        #endregion
    }
}
