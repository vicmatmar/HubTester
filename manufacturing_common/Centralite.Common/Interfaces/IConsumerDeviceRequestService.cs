using Centralite.Common.Models;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which defines how to request a new device
    /// </summary>
    public interface IConsumerDeviceRequestService
    {
        /// <summary>
        /// Method called by the consumer when a new device needs to be created
        /// </summary>
        /// <param name="address"></param>
        /// <returns><see cref="ZigbeeDeviceBase"/></returns>
        ZigbeeDeviceBase RequestDevice(ushort address);
    }
}
