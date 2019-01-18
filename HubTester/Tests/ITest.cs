using System.ComponentModel;
using System.Threading;

namespace HubTests.Tests
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
