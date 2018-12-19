using Centralite.Common.Interfaces;
using System.ComponentModel.Composition;
using Centralite.Common.Models;
using Centralite.Ember.Ezsp;

namespace Centralite.Common
{
    public abstract class BasePluginViewModel : ValidationBindableBase
    {
        protected IProducerDeviceRequestService deviceRequestService;
        protected IConsumerMessageQueueService messageQueueService;
        protected ITestService testService;
        protected IPluginPrinterService printerService;
        protected IEzspService ezspService;
        protected IErrorProducerService errorProducerService;

        [ImportingConstructor]
        public BasePluginViewModel(IProducerDeviceRequestService deviceRequestService, IConsumerMessageQueueService messageQueueService, ITestService testService, IPluginPrinterService printerService, IEzspService ezspService, IErrorProducerService errorProducerService)
        {
            this.deviceRequestService = deviceRequestService;
            this.messageQueueService = messageQueueService;
            this.testService = testService;
            this.printerService = printerService;
            this.ezspService = ezspService;
            this.errorProducerService = errorProducerService;
        }

        public virtual void InitializeServices()
        {
            this.deviceRequestService.ClearProducerEvents();
            this.deviceRequestService.OnDeviceRequest += DeviceRequestService_OnDeviceRequest;

            this.messageQueueService.ClearConsumerEvents();
            this.messageQueueService.MessageAddedEvent += MessageQueueService_MessageAddedEvent;

            this.testService.ClearTestEvents();
            this.testService.OnDeviceJoined += TestService_OnDeviceJoined;
            this.testService.OnDeviceLeft += TestService_OnDeviceLeft;
        }

        protected virtual void MessageQueueService_MessageAddedEvent()
        {
            var message = messageQueueService.RetrieveMessage();

            if (message != null)
            {
                ProcessMessage(message.Item1, message.Item2);
            }
        }

        protected abstract void TestService_OnDeviceLeft(ZigbeeDeviceBase device);
        protected abstract void TestService_OnDeviceJoined(ZigbeeDeviceBase device);
        protected abstract ZigbeeDeviceBase DeviceRequestService_OnDeviceRequest(ushort arg);
        protected abstract void ProcessMessage(EzspIncomingMessageHandlerResponse message, ZigbeeDeviceBase device);
    }
}
