using Centralite.Common.Models;
using Centralite.Database;
using Centralite.Ember.Ezsp;
using System;

namespace Centralite.Common.Interfaces
{
    public interface IDeviceValidationService
    {
        Product ValidProduct { get; set; }
        StationSite CurrentStationSite { get; set; }

        void ValidateDevice(ZigbeeDeviceBase device, EzspIncomingMessageHandlerResponse message);

        event Action<ZigbeeDeviceBase> OnAddDevice;
        event Action<ZigbeeDeviceBase> OnRemoveDevice;
    }
}
