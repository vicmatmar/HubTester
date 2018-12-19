using Centralite.Common.Models;
using System;

namespace Centralite.Common.Interfaces
{
    public interface ITestFinalizeService
    {
        void ClearEvents();

        void LabelPrinted(ZigbeeDeviceBase device, int testerId, int networkColorId, int productionSiteId);
        void SerialNumberGenerated(ZigbeeDeviceBase device, int? testerId, int? supervisorId);
        void SerialNumberUpdated(ZigbeeDeviceBase device, int? testerId);

        event Action<ZigbeeDeviceBase, int?, int?> OnSerialNumberGenerated;
        event Action<ZigbeeDeviceBase, int?> OnSerialNumberUpdated;
        event Action<ZigbeeDeviceBase, int, int, int> OnLabelPrinted;
    }
}
