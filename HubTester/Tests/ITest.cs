using System.ComponentModel;

namespace HubTests.Tests
{
    public interface ITest: INotifyPropertyChanged
    {
        TestStatus TestStatus { get; }

        bool Setup();
        bool Run();
        bool TearDown();
    }
}
