using System;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using System.ComponentModel.Composition;

namespace Centralite.Services
{
    [Export(typeof(IConsumerDeviceRequestService))]
    [Export(typeof(IProducerDeviceRequestService))]
    public class DeviceRequestService : IConsumerDeviceRequestService, IProducerDeviceRequestService
    {
        public event Func<ushort, ZigbeeDeviceBase> OnDeviceRequest;

        public void ClearProducerEvents()
        {
            OnDeviceRequest = null;
        }

        public ZigbeeDeviceBase RequestDevice(ushort address)
        {
            return OnDeviceRequest?.Invoke(address);
        }
    }
}
