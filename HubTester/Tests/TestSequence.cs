using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HubTester.Tests
{
    public class TestSequence: INotifyPropertyChanged
    {
        private readonly List<ITest> _tests = new List<ITest>();
        public List<ITest> Tests { get => _tests; }

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
                NotifyPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessors of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
