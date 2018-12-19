using Centralite.Common.Models;
using Centralite.Ember.Ezsp;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which defines the producer part of the MessageQueueService
    /// Consumer part is <see cref="IConsumerMessageQueueService"/>
    /// </summary>
    public interface IProducerMessageQueueService
    {
        /// <summary>
        /// Producer calls this method when they have a new message which needs to be processed by the consumer
        /// </summary>
        /// <param name="message"></param>
        /// <param name="device"></param>
        void AddMessage(EzspIncomingMessageHandlerResponse message, ZigbeeDeviceBase device);
    }
}
