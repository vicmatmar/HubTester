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
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        IProgress<TestStatus> _progress;

        CancellationTokenSource test_cancel_ts;


        string _ipaddress = "192.168.10.2";
        protected string Ipaddress { get => _ipaddress; }

        private List<ITest> _tests = new List<ITest>();
        protected List<ITest> Tests { get => _tests; }

        bool _testsLoaded = false;
        protected bool TestsLoaded { get => _testsLoaded; set => _testsLoaded = value; }

        int _testIndex = 0;
        protected int TestIndex { get => _testIndex; set => _testIndex = value; }

        protected bool TestSequenceRunning { get; set; }

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
            Tests.Add(test);
        }

        private void LoadTests()
        {

            AddTest(new EmberTest(Properties.Settings.Default.TestEui));

            AddTest(new LedTest());
            AddTest(new TamperTest());
            AddTest(new BuzzerTest("Is Buzzer Active?"));

            AddTest(new UsbTest());

            //AddTest(new BluetoothTest());
            AddTest(new ZwaveTest());


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

            TestStatus ts;
            while (TestIndex < Tests.Count)
            {
                TestSequenceRunning = true;

                bool setupPassed = false;
                bool runPassed = false;
                bool tearDownPassed = false;

                ITest test = Tests[TestIndex];
                test.CancelToken = test_cancel_ts.Token;

                _logger.Debug($"Run Test Index {TestIndex} of {Tests.Count}: {test.GetType().Name}");

                ts = new TestStatus();
                ts.Test = test;
                ts.PropertyName = TestStatusPropertyNames.Status;
                ts.Status = $"{test.GetType().Name} ({TestIndex + 1}/{Tests.Count})";
                ts.Exception = null;
                progress.Report(ts);

                try
                {
                    setupPassed = test.Setup();

                    if (!setupPassed)
                    {
                        ts.Status = test.GetType().Name + " Setup Failed.";
                        ts.PropertyName = TestStatusPropertyNames.ErrorMsg;
                        progress.Report(ts);
                    }
                }
                catch (Exception ex)
                {
                    setupPassed = false;
                    ts.Status = test.GetType().Name + " Setup Exception";
                    ts.PropertyName = TestStatusPropertyNames.Exception;
                    ts.Exception = ex;
                    progress.Report(ts);
                }

                // If setup fails, no reason to run test
                if (setupPassed && !test_cancel_ts.IsCancellationRequested)
                {
                    try
                    {
                        runPassed = test.Run();
                        if (!runPassed)
                        {
                            ts.Status = test.GetType().Name + " Run Failed";
                            ts.PropertyName = TestStatusPropertyNames.ErrorMsg;
                            progress.Report(ts);
                        }

                    }
                    catch (Exception ex)
                    {
                        runPassed = false;
                        ts.Status = test.GetType().Name + " Run Exception";
                        ts.PropertyName = TestStatusPropertyNames.Exception;
                        ts.Exception = ex;
                        progress.Report(ts);
                    }
                }

                try
                {
                    tearDownPassed = test.TearDown();
                    if (!tearDownPassed)
                    {
                        ts.Status = test.GetType().Name + " Teardown Failure.";
                        ts.PropertyName = TestStatusPropertyNames.ErrorMsg;
                        progress.Report(ts);
                    }
                }
                catch (Exception ex)
                {
                    tearDownPassed = false;

                    ts.Status = test.GetType().Name + " Teardown Exception";
                    ts.PropertyName = TestStatusPropertyNames.Exception;
                    ts.Exception = ex;
                    progress.Report(ts);
                }

                if (setupPassed && runPassed && tearDownPassed)
                {
                    ts = new TestStatus();
                    ts.Status = $"Test Passed\r\n";
                    ts.PropertyName = TestStatusPropertyNames.Status;
                    progress.Report(ts);

                    // next test
                    TestIndex++;
                }
                else
                {
                    ts = new TestStatus();
                    ts.Status = $"Test Failed\r\n";
                    ts.PropertyName = TestStatusPropertyNames.Status;
                    progress.Report(ts);

                    break;
                }
            }

            if (TestIndex >= Tests.Count || test_cancel_ts.IsCancellationRequested)
            {
                // Reset TestIndex to run all tests
                TestIndex = 0;
                TestSequenceRunning = false;

                if (!test_cancel_ts.IsCancellationRequested)
                {
                    ts = new TestStatus();
                    ts.Status = $"All tests passed successfully";
                    ts.PropertyName = TestStatusPropertyNames.Status;
                    progress.Report(ts);
                }
            }
        }

        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITest test = (ITest)sender;

            if (e.PropertyName == TestStatusPropertyNames.ShowQuestionDlg.ToString())
            {
                test.TestStatus.ShowQuestionDlg.DialogResult = DialogResult.None;

                _progress.Report(test.TestStatus);

                // TODO: This is ugly
                while (test.TestStatus.ShowQuestionDlg.DialogResult == DialogResult.None) ;

                TestStatus t = test.TestStatus;
            }
            else
            {
                _progress.Report(test.TestStatus);
            }
        }

        private async void RunButton_Click(object sender, EventArgs e)
        {
            _logger.Debug("Run button clicked");

            test_cancel_ts = new CancellationTokenSource();

            runButton.Enabled = false;
            cancelButton.Enabled = true;

            if (TestIndex == 0)
            {
                runTextBox.Clear();
            }

            var progress =
                new Progress<TestStatus>(s =>
                    {
                        string timestamp_str = DateTime.Now.ToString("hh:mm:ss");

                        if (s.Test != null)
                            _logger.Debug($"{s.Test.GetType().Name}: {s.Status}");
                        else
                            _logger.Debug($"Status: {s.Status}");

                        switch (s.PropertyName)
                        {
                            case TestStatusPropertyNames.Status:
                                runTextBox.AppendText($"{timestamp_str}: {s.Status}\r\n");
                                break;
                            case TestStatusPropertyNames.ErrorMsg:
                                runTextBox.AppendText($"{timestamp_str}: {s.Status}\r\n");
                                break;
                            case TestStatusPropertyNames.Exception:
                                _logger.Error(s.Exception, s.Test.GetType().Name);

                                runTextBox.AppendText($"{timestamp_str}: {s.Status}\r\n");
                                runTextBox.AppendText($"{s.Exception.Message}\r\n\r\n");
                                runTextBox.AppendText($"{s.Exception.StackTrace}\r\n");

                                break;
                            case TestStatusPropertyNames.ShowQuestionDlg:
                                ShowQuestionDlg dlgt = s.ShowQuestionDlg;

                                _logger.Debug($"{s.Test.GetType().Name} show dialog: {dlgt.Text}, {dlgt.Caption}, {dlgt.Btns.ToString()}");

                                dlgt.DialogResult = MessageBoxEx.Show(this, dlgt.Text, dlgt.Caption, dlgt.Btns);
                                s.ShowQuestionDlg.ShowDialog = false;

                                break;
                            default:
                                runTextBox.AppendText($"{timestamp_str}: Unhandled PropertyName\r\n");
                                break;
                        }
                    }
                );

            await Task.Run(() => RunTests(progress));


            if (TestSequenceRunning)
            {
                runButton.Text = "&Rerun";
            }
            else
            {
                runButton.Text = "&Run";
                cancelButton.Enabled = false;
            }

            runButton.Enabled = true;

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            cancelButton.Enabled = false;

            test_cancel_ts.Cancel();

            runButton.Text = "&Run";
            TestIndex = 0;

        }
    }
}

