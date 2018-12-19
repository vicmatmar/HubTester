using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using Centralite.Ember.Ezsp;

namespace Centralite.Services
{
    [Export(typeof(IProducerMessageQueueService))]
    [Export(typeof(IConsumerMessageQueueService))]
    public class MessageQueueService : IProducerMessageQueueService, IConsumerMessageQueueService
    {
        private Queue<Tuple<EzspIncomingMessageHandlerResponse, ZigbeeDeviceBase>> Messages = new Queue<Tuple<EzspIncomingMessageHandlerResponse, ZigbeeDeviceBase>>();

        public event Action MessageAddedEvent;

        public void AddMessage(EzspIncomingMessageHandlerResponse message, ZigbeeDeviceBase device)
        {
            lock (Messages)
            {
                Messages.Enqueue(Tuple.Create(message, device));
            }

            MessageAddedEvent?.Invoke();
        }

        public void ClearConsumerEvents()
        {
            MessageAddedEvent = null;
        }

        public Tuple<EzspIncomingMessageHandlerResponse, ZigbeeDeviceBase> RetrieveMessage()
        {
            lock (Messages)
            {
                if (Messages.Count != 0)
                {
                    return Messages.Dequeue();
                }
                else
                {
                    return default(Tuple<EzspIncomingMessageHandlerResponse, ZigbeeDeviceBase>);
                }
            }
        }
    }
}
