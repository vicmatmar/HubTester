using System.ComponentModel;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public interface ITest: INotifyPropertyChanged
    {
        TestStatus TestStatus { get; }

        Form ParentForm { get; set; }

        bool Setup();
        bool Run();
        bool TearDown();
    }
}
