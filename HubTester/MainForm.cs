using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using HubTests.Tests;

namespace HubTester
{
    public partial class MainForm : Form
    {

        IProgress<TestStatus> _progress;

        string _ipaddress = "192.168.10.2";
        protected string Ipaddress { get => _ipaddress; }

        private List<ITest> _tests = new List<ITest>();
        protected List<ITest> Tests { get => _tests; }

        bool _testsLoaded = false;
        protected bool TestsLoaded { get => _testsLoaded; set => _testsLoaded = value; }

        int _testIndex = 0;
        protected int TestIndex { get => _testIndex; set => _testIndex = value; }

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        void AddTest(ITest test)
        {
            test.PropertyChanged -= Test_PropertyChanged;
            test.PropertyChanged += Test_PropertyChanged;
            test.ParentForm = this;
            Tests.Add(test);
        }

        private void LoadTests()
        {

            AddTest(new LedTest());
            AddTest(new TamperTest());
            AddTest(new BuzzerTest("Is Buzzer Active?"));

            //tests.Add(new UsbTest(IpAddress, RsaFile, "Insert USB to first USB Slot"));
            //tests.Add(new UsbTest(IpAddress, RsaFile, "Insert USB to second USB Slot"));

            //AddTest(new BluetoothTest());
            AddTest(new ZwaveTest());

            //tests.Add(new EmberTest(IpAddress, RsaFile, TestEui));

            // Generate next MAC address and write to board
            //tests.Add(new MacTest(IpAddress, RsaFile, StartBlock, EndBlock));

            // Generate Activation Key and read from board
            //tests.Add(new ActivationTest(IpAddress, RsaFile));

            // Battery Test can cause the hub to reboot so make it final test
            //tests.Add(new BatteryTest(IpAddress, RsaFile));

            // Print Final Label
            //tests.Add(new PrintLabel(HubLabelPrinterAddress, ActivationCodePrinterAddress));

            TestsLoaded = true;
        }

        private void UnloadTests()
        {
            Tests.Clear();
            TestsLoaded = false;
        }

        private void RunTests(IProgress<TestStatus> progress)
        {
            _progress = progress;


            if (!TestsLoaded)
                LoadTests();

            // Tests have already ran and passed
            // Restart Testing from beginning
            if (TestIndex >= Tests.Count)
            {
                TestIndex = 0;
            }

            while (TestIndex < Tests.Count)
            {
                bool testPassed = true;

                ITest test = Tests[TestIndex];

                TestStatus reportTestStatus = new TestStatus
                {
                    Status = test.GetType().Name,
                    Exception = null
                };
                progress.Report(reportTestStatus);

                testPassed &= test.Setup();

                // If setup fails, no reason to run test
                if (testPassed)
                {
                    try
                    {
                        testPassed &= test.Run();
                    }
                    catch (Exception ex)
                    {
                        testPassed &= false;
                        test.TestStatus.Exception = ex;
                    }
                    if (!testPassed && test.TestStatus.Exception != null)
                    {
                        reportTestStatus.Status = test.GetType().Name + " Run Exception";
                        reportTestStatus.Exception = test.TestStatus.Exception;
                        progress.Report(reportTestStatus);
                    }
                }

                try
                {
                    testPassed &= test.TearDown();
                }
                catch (Exception ex)
                {
                    testPassed &= false;
                    test.TestStatus.Exception = ex;
                    reportTestStatus.Status = test.GetType().Name + " Teardown Exception";
                    reportTestStatus.Exception = test.TestStatus.Exception;
                    progress.Report(reportTestStatus);
                }


                progress.Report(new TestStatus { Status = "\n" });

                if (testPassed)
                {
                    TestIndex++;
                }
                else
                {
                    break;
                }
            }

            // All tests ran successfully
            // Reset TestIndex to run all tests
            if (TestIndex >= Tests.Count)
            {
                TestIndex = 0;
                progress.Report(new TestStatus { Status = "All tests passed successfully" });
                //TestStatus = "All tests passed successfully";
            }
        }

        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITest test = (ITest)sender;

            if (e.PropertyName == "ShowQuestionDiag")
            {
                test.TestStatus.ShowQuestionDig.DialogResult = DialogResult.None;

                _progress.Report(test.TestStatus);

                // TODO: This is ugly
                while (test.TestStatus.ShowQuestionDig.DialogResult == DialogResult.None) ;

                TestStatus t = test.TestStatus;
            }
            else
            {
                _progress.Report(test.TestStatus);
            }
        }

        private async void RunButton_Click(object sender, EventArgs e)
        {
            RunButton.Enabled = false;

            if (TestIndex == 0)
                runTextBox.Clear();

            var progress =
                new Progress<TestStatus>(s =>
                    {
                        if (s.ShowQuestionDig != null && s.ShowQuestionDig.ShowDialog)
                        {
                            ShowQuestionDlg dlgt = s.ShowQuestionDig;
                            dlgt.DialogResult = MessageBoxEx.Show(this, dlgt.Text, dlgt.Caption, dlgt.Btns);
                            s.ShowQuestionDig.ShowDialog = false;
                        }
                        else
                        {
                            runTextBox.AppendText(s.Status + "\r\n");

                            if (s.Exception != null)
                                runTextBox.AppendText(s.Exception.Message + "\r\n" + s.Exception.StackTrace + "\r\n");
                        }
                    }
                );

            await Task.Run(() => RunTests(progress));

            RunButton.Enabled = true;

        }
    }
}
