using Centralite.Common.Models;
using System;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which defines how to create a new device when requested
    /// </summary>
    public interface IProducerDeviceRequestService
    {
        /// <summary>
        /// Event which should be subscribed to by the producer, which should be called when a new device needs to be created by the plugin.
        /// </summary>
        event Func<ushort, ZigbeeDeviceBase> OnDeviceRequest;

        /// <summary>
        /// Clears all registered event delegates. Used to ensure that when configuring the service, that no other events are registered.
        /// </summary>
        void ClearProducerEvents();
    }
}
