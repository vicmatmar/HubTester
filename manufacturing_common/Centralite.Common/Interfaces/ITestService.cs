using Centralite.Common.Models;
using System;
using System.Collections.Generic;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which allows for getting Host Application's Devices to perform actions on
    /// Exposes two events that can be subscribed to, which trigger when a device leaves or joins
    /// Works in coordination with <see cref="IHostService"/>
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// Gets all devices from Host Application
        /// </summary>
        /// <returns><see cref="IEnumerable{ZigbeeDeviceBase}"/></returns>
        IEnumerable<ZigbeeDeviceBase> RequestDevices();

        /// <summary>
        /// Event which is called when a Device joins
        /// </summary>
        /// <param><see cref="ZigbeeDeviceBase"/></param>
        event Action<ZigbeeDeviceBase> OnDeviceJoined;

        /// <summary>
        /// Event which is called when a Device leaves
        /// </summary>
        /// <param><see cref="ZigbeeDeviceBase"/></param>
        event Action<ZigbeeDeviceBase> OnDeviceLeft;

        /// <summary>
        /// Clears all registered event delegates. Used to ensure that when configuring the service, that no other events are registered.
        /// </summary>
        void ClearTestEvents();
    }
}
