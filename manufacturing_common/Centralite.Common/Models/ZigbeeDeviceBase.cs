using Centralite.Common.Enumerations;
using Centralite.Ember.DataTypes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Centralite.Common.Models
{
    public abstract class ZigbeeDeviceBase : INotifyPropertyChanged
    {
        protected ushort address;
        public ushort Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                RaisePropertyChanged();
            }
        }

        protected EmberEui64 eui;
        public EmberEui64 Eui
        {
            get
            {
                return eui;
            }
            set
            {
                eui = value;
                RaisePropertyChanged();
            }
        }

        protected string modelString;
        public string ModelString
        {
            get
            {
                return modelString;
            }
            set
            {
                modelString = value;
                RaisePropertyChanged();
            }
        }

        public ushort FirmwareVersion
        {
            get
            {
                return (ushort)(LongFirmwareVersion >> 16);
            }
        }

        protected uint longFirmwareVersion;
        public uint LongFirmwareVersion
        {
            get
            {
                return longFirmwareVersion;
            }
            set
            {
                longFirmwareVersion = value;
                RaisePropertyChanged();
                RaisePropertyChanged("FirmwareVersion");
            }
        }

        protected bool sleepy;
        public bool Sleepy
        {
            get
            {
                return sleepy;
            }
            private set
            {
                sleepy = value;
                RaisePropertyChanged();
            }
        }

        protected bool validated;
        public bool Validated
        {
            get
            {
                return validated;
            }
            set
            {
                validated = value;
                RaisePropertyChanged();
            }
        }

        public abstract string DeviceStatus { get; }

        protected DevicePrintStatus printStatus = DevicePrintStatus.Default;
        public DevicePrintStatus PrintStatus
        {
            get
            {
                return printStatus;
            }
            set
            {
                printStatus = value;
                RaisePropertyChanged();
            }
        }

        private bool serialGenerated = false;
        public bool SerialGenerated
        {
            get
            {
                return serialGenerated;
            }
            set
            {
                serialGenerated = value;
                RaisePropertyChanged();
            }
        }

        public ZigbeeDeviceBase(bool sleepy)
        {
            this.Sleepy = sleepy;
        }

        public abstract bool TestsPassed();

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
