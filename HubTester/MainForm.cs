using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        CancellationTokenSource testCancelTs;
        Stopwatch sequenceStopWatch = new Stopwatch();


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

            AddTest(new EthernetTest(120));

            AddTest(new BuzzerTest());

            AddTest(new LedTest());
            AddTest(new TamperTest());

            AddTest(new UsbTest());

            //AddTest(new BluetoothTest());
            AddTest(new ZwaveTest());

            AddTest(new EmberTest(Properties.Settings.Default.TestEui));


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

            if(TestIndex == 0)
                sequenceStopWatch.Restart();

            Stopwatch testStopWatch = new Stopwatch();

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
                testStopWatch.Restart();

                TestSequenceRunning = true;

                bool setupPassed = false;
                bool runPassed = false;
                bool tearDownPassed = false;

                ITest test = Tests[TestIndex];
                test.CancelToken = testCancelTs.Token;

                _logger.Debug($"Run Test Index {TestIndex} of {Tests.Count}: {test.GetType().Name}");

                ts = new TestStatus(test, $"{test.GetType().Name} ({TestIndex + 1}/{Tests.Count})");
                progress.Report(ts);

                try
                {
                    setupPassed = test.Setup();

                    if (!setupPassed)
                    {
                        string msg = " Setup " + (testCancelTs.Token.IsCancellationRequested ? "Canceled" : "Failed");
                        ts = new TestStatus(test, test.GetType().Name + msg);
                        progress.Report(ts);
                    }
                }
                catch (Exception ex)
                {
                    setupPassed = false;

                    ts = new TestStatus(test, TestStatusPropertyNames.Exception)
                    {
                        Status = test.GetType().Name + " Setup Exception",
                        Exception = ex
                    };
                    progress.Report(ts);
                }

                // If setup fails or canceled, no reason to run test
                if (setupPassed && !testCancelTs.IsCancellationRequested)
                {
                    try
                    {
                        runPassed = test.Run();
                        if (!runPassed)
                        {
                            string msg = " Run " + (testCancelTs.Token.IsCancellationRequested ? "Canceled" : "Failed");
                            ts = new TestStatus(test, test.GetType().Name + msg);
                            progress.Report(ts);
                        }

                    }
                    catch (Exception ex)
                    {
                        runPassed = false;

                        ts = new TestStatus(test, TestStatusPropertyNames.Exception)
                        {
                            Status = test.GetType().Name + " Run Exception",
                            Exception = ex
                        };
                        progress.Report(ts);
                    }
                }

                try
                {
                    tearDownPassed = test.TearDown();
                    if (!tearDownPassed)
                    {
                        ts = new TestStatus(test)
                        {
                            Status = test.GetType().Name + " Teardown Failure."
                        };
                        progress.Report(ts);
                    }
                }
                catch (Exception ex)
                {
                    tearDownPassed = false;

                    ts = new TestStatus(test, TestStatusPropertyNames.Exception)
                    {
                        Status = test.GetType().Name + " Teardown Exception",
                        Exception = ex
                    };
                    progress.Report(ts);
                }

                testStopWatch.Stop();
                string etimestr = $"({testStopWatch.Elapsed.ToString(@"m\:ss")})";

                if (setupPassed && runPassed && tearDownPassed)
                {
                    ts = new TestStatus(test, $"Test Passed {etimestr}\r\n");
                    progress.Report(ts);

                    // next test
                    TestIndex++;
                }
                else
                {
                    ts = new TestStatus(test, $"Test Failed {etimestr}\r\n");
                    progress.Report(ts);

                    break;
                }
            }

            if (TestIndex >= Tests.Count || testCancelTs.IsCancellationRequested)
            {
                // Reset TestIndex to run all tests
                TestIndex = 0;
                TestSequenceRunning = false;

                sequenceStopWatch.Stop();
                string etimestr = $" ({sequenceStopWatch.Elapsed.ToString(@"m\:ss")})";

                ts = new TestStatus{
                    PropertyName = TestStatusPropertyNames.Status,
                };
                ts.Status = testCancelTs.IsCancellationRequested?
                    $"Test sequence canceled":$"All tests passed successfully";
                ts.Status += etimestr;
                progress.Report(ts);
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

            testCancelTs = new CancellationTokenSource();

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
                                runTextBox.AppendText($"{timestamp_str}: {s.ErrorMsg}\r\n");
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
                            case TestStatusPropertyNames.HUB_EUI:
                                string euistr = s.Status;
                                long hub_eui = Convert.ToInt64(euistr, 16);
                                runTextBox.AppendText($"{timestamp_str}: HUB EUI LONG: {hub_eui}\r\n");
                                break;
                            default:
                                runTextBox.AppendText($"{timestamp_str}: Unhanded PropertyName\r\n");
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

        private void CancelButton_Click(object sender, EventArgs e)
        {
            cancelButton.Enabled = false;

            testCancelTs.Cancel();

            runButton.Text = "&Run";
            TestIndex = 0;

        }
    }
}

