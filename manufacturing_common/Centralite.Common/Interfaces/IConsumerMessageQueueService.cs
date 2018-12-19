using System;
using Centralite.Common.Models;
using Centralite.Ember.Ezsp;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which defines the consumer part of the MessageQueueService
    /// Producer part is <see cref="IProducerMessageQueueService"/>
    /// </summary>
    public interface IConsumerMessageQueueService
    {
        /// <summary>
        /// Consumer calls this method when they are ready to consume a message
        /// </summary>
        /// <returns></returns>
        Tuple<EzspIncomingMessageHandlerResponse, ZigbeeDeviceBase> RetrieveMessage();

        /// <summary>
        /// Event used by the Consumer to be notified when a new message has been added to be consumed
        /// </summary>
        event Action MessageAddedEvent;

        /// <summary>
        /// Clears all registered event delegates. Used to ensure that when configuring the service, that no other events are registered.
        /// </summary>
        void ClearConsumerEvents();
    }
}
