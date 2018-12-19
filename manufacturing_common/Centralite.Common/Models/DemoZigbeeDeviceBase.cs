using System;

namespace Centralite.Common.Models
{
    public class DemoZigbeeDeviceBase : ZigbeeDeviceBase
    {
        public DemoZigbeeDeviceBase() : base(false) { }

        public override string DeviceStatus
        {
            get
            {
                return null;
            }
        }

        public override bool TestsPassed()
        {
            return true;
        }
    }
}
