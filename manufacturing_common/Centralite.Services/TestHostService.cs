using Centralite.Common.Interfaces;
using System;
using System.Collections.Generic;
using Centralite.Common.Models;
using System.ComponentModel.Composition;

namespace Centralite.Services
{
    [Export(typeof(ITestService))]
    [Export(typeof(IHostService))]
    public class TestHostService : ITestService, IHostService
    {
        public event Action<ZigbeeDeviceBase> OnDeviceJoined;
        public event Action<ZigbeeDeviceBase> OnDeviceLeft;
        public event Func<IEnumerable<ZigbeeDeviceBase>> OnDevicesRequested;

        public void ClearHostEvents()
        {
            OnDevicesRequested = null;
        }

        public void ClearTestEvents()
        {
            OnDeviceJoined = null;
            OnDeviceLeft = null;
        }

        public void DeviceJoined(ZigbeeDeviceBase device)
        {
            OnDeviceJoined?.Invoke(device);
        }

        public void DeviceLeft(ZigbeeDeviceBase device)
        {
            OnDeviceLeft?.Invoke(device);
        }

        public IEnumerable<ZigbeeDeviceBase> RequestDevices()
        {
            return OnDevicesRequested?.Invoke();
        }
    }
}
