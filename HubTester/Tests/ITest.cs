using System.ComponentModel;

namespace HubTester.Tests
{
    public interface ITest: INotifyPropertyChanged
    {
        TestStatus TestStatus { get; }

        bool Setup();
        bool Run();
        bool TearDown();

        void Cancel();
        bool IsCancellationRequested { get; }
    }
}
