using Centralite.Common.Models;
using System;
using System.Collections.Generic;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which exposes the ability to get all Devices and calls events when a device joins or leaves
    /// Works in coordination <see cref="ITestService"/>
    /// </summary>
    public interface IHostService
    {
        /// <summary>
        /// Call when a device joins
        /// </summary>
        void DeviceJoined(ZigbeeDeviceBase device);

        /// <summary>
        /// Call when a device leaves
        /// </summary>
        void DeviceLeft(ZigbeeDeviceBase device);

        /// <summary>
        /// Event which is called when Devices are requested
        /// </summary>
        event Func<IEnumerable<ZigbeeDeviceBase>> OnDevicesRequested;

        /// <summary>
        /// Clears all registered event delegates. Used to ensure that when configuring the service, that no other events are registered.
        /// </summary>
        void ClearHostEvents();
    }
}
