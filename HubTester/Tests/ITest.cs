using System.ComponentModel;
using System.Threading;

namespace HubTests.Tests
{
    public interface ITest: INotifyPropertyChanged
    {
        TestStatus TestStatus { get; }

        CancellationToken CancelToken { set; }

        bool Setup();
        bool Run();
        bool TearDown();
    }
}
