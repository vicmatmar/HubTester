using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace HubTester.Tests
{
    [DataContract]
    [KnownType(typeof(ZwaveTest))]
    [KnownType(typeof(UsbTest))]
    [KnownType(typeof(TamperTest))]
    [KnownType(typeof(LedTest))]
    [KnownType(typeof(EmberTest))]
    [KnownType(typeof(BuzzerTest))]
    [KnownType(typeof(EthernetTest))]
    [KnownType(typeof(MacTest))]
    public class TestSequence: INotifyPropertyChanged
    {
        public TestSequence()
        {

        }
        
        List<ITest> _tests = new List<ITest>();
        [DataMember]
        public List<ITest> Tests { get => _tests; set => _tests = value; }

        public int Count { get { return Tests.Count; } }

        public void Clear()
        {
            Tests.Clear();
        }

        public void Add(ITest test)
        {
            Tests.Add(test);
        }

        bool _testSequenceRunning = false;
        public bool IsRunning
        {
            get { return _testSequenceRunning; }
            set
            {
                _testSequenceRunning = value;
                OnPropertyChanged();
            }
        }

        string _hub_eui = null;
        public string HUB_EUI
        {
            get
            {
                return _hub_eui;
            }
            set
            {
                _hub_eui = value;
                OnPropertyChanged();
            }
        }

        string _hub_mac_addr = null;
        public string HUB_MAC_ADDR
        {
            get
            {
                return _hub_mac_addr;
            }
            set
            {
                _hub_mac_addr = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
