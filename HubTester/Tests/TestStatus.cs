using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class TestStatus
    {
        public string Status { get; set; }
        public Exception Exception { get; set; }

        public ShowQuestionDiag ShowQuestionDig { get; set; }
    }

    public class ShowQuestionDiag
    {
        string _text;
        string _caption;
        MessageBoxButtons _btns;

        public ShowQuestionDiag(string text, string caption, MessageBoxButtons btns)
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
