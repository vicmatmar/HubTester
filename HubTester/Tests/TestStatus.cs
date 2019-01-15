using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace HubTests.Tests
{
    public enum TestStatusPropertyNames { Status, ErrorMsg, Exception, ShowQuestionDlg, HUB_EUI };

    public class TestStatus
    {
        private string _status = "";
        private string _errorMsg = "";
        private TestStatusPropertyNames _propertyName = TestStatusPropertyNames.Status;

        public TestStatus()
        {
        }

        public TestStatus(ITest test, TestStatusPropertyNames propertyName = TestStatusPropertyNames.Status)
        {
            this.Test = test;
            this.PropertyName = propertyName;
        }

        public TestStatus(ITest test, string status)
        {
            this.Test = test;
            this.Status = status;
        }

        public TestStatusPropertyNames PropertyName { get => _propertyName; set => _propertyName = value; }
        public string Status { get => _status; set => _status = value; }
        public string ErrorMsg { get => _errorMsg; set => _errorMsg = value; }

        public Exception Exception { get; set; }

        public ShowQuestionDlg ShowQuestionDlg { get; set; }

        public ITest Test { get; set; }
    }

    public class ShowQuestionDlg
    {
        string _text;
        string _caption;
        MessageBoxButtons _btns;

        public ShowQuestionDlg(string text, string caption, MessageBoxButtons btns)
        {
            Text = text;
            Caption = caption;
            Btns = btns;
            ShowDialog = true;
        }

        public string Text { get => _text; set => _text = value; }
        public string Caption { get => _caption; set => _caption = value; }
        public MessageBoxButtons Btns { get => _btns; set => _btns = value; }
        public DialogResult DialogResult { get; set; }
        public bool ShowDialog { get; set; }
    }
}
