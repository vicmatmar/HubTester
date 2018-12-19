using Centralite.Common.Interfaces;
using System;
using Centralite.Common.Models;
using System.ComponentModel.Composition;

namespace Centralite.Services
{
    [Export(typeof(ITestFinalizeService))]
    public class TestFinalizeService : ITestFinalizeService
    {
        public void ClearEvents()
        {
            OnLabelPrinted = null;
            OnSerialNumberGenerated = null;
            OnSerialNumberUpdated = null;
        }

        public void LabelPrinted(ZigbeeDeviceBase device, int testerId, int networkColorId, int productionSiteId)
        {
            OnLabelPrinted?.Invoke(device, testerId, networkColorId, productionSiteId);
        }

        public void SerialNumberGenerated(ZigbeeDeviceBase device, int? testerId, int? supervisorId)
        {
            OnSerialNumberGenerated?.Invoke(device, testerId, supervisorId);
        }

        public void SerialNumberUpdated(ZigbeeDeviceBase device, int? testerId)
        {
            OnSerialNumberUpdated?.Invoke(device, testerId);
        }

        public event Action<ZigbeeDeviceBase, int, int, int> OnLabelPrinted;
        public event Action<ZigbeeDeviceBase, int?, int?> OnSerialNumberGenerated;
        public event Action<ZigbeeDeviceBase, int?> OnSerialNumberUpdated;
    }
}
