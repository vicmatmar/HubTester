﻿using System;
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

            AddTest(new LedTest());
            AddTest(new TamperTest());
            AddTest(new BuzzerTest("Is Buzzer Active?"));

            AddTest(new UsbTest());

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
                TestSequenceRunning = true;

                bool testPassed = true;

                ITest test = Tests[TestIndex];
                test.CancelToken = test_cancel_ts.Token;

                _logger.Debug($"Run Test Index {TestIndex}: {test.GetType().Name}");


                TestStatus reportTestStatus = new TestStatus
                {
                    Test = test,
                    Status = test.GetType().Name,
                    Exception = null
                };
                progress.Report(reportTestStatus);

                try
                {
                    testPassed &= test.Setup();
                }
                catch (Exception ex)
                {
                    testPassed &= false;
                    reportTestStatus.Status = test.GetType().Name + " Setup Exception";
                    reportTestStatus.Exception = ex;
                    progress.Report(reportTestStatus);
                }

                // If setup fails, no reason to run test
                if (testPassed && !test_cancel_ts.IsCancellationRequested)
                {
                    try
                    {
                        testPassed &= test.Run();
                    }
                    catch (Exception ex)
                    {
                        testPassed &= false;
                        reportTestStatus.Status = test.GetType().Name + " Run Exception";
                        reportTestStatus.Exception = ex;
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
                    reportTestStatus.Status = test.GetType().Name + " Teardown Exception";
                    reportTestStatus.Exception = ex;
                    progress.Report(reportTestStatus);
                }



                if (testPassed)
                {
                    reportTestStatus.Status = "Test Passed\r\n";
                    progress.Report(reportTestStatus);

                    TestIndex++;
                }
                else
                {
                    reportTestStatus.Status = "Test Failed\r\n";
                    reportTestStatus.StatusColor = Color.Red;
                    progress.Report(reportTestStatus);

                    break;
                }
            }

            if (TestIndex >= Tests.Count || test_cancel_ts.IsCancellationRequested)
            {
                // Reset TestIndex to run all tests
                TestIndex = 0;
                TestSequenceRunning = false;

                if (!test_cancel_ts.IsCancellationRequested)
                    progress.Report(new TestStatus { Status = "All tests passed successfully" });
            }
        }

        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITest test = (ITest)sender;

            if (e.PropertyName == "ShowQuestionDiag")
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
                        if (s.ShowQuestionDlg != null && s.ShowQuestionDlg.ShowDialog)
                        {
                            ShowQuestionDlg dlgt = s.ShowQuestionDlg;

                            _logger.Debug($"{s.Test.GetType().Name} show dialog: {dlgt.Text}, {dlgt.Caption}, {dlgt.Btns.ToString()}");

                            dlgt.DialogResult = MessageBoxEx.Show(this, dlgt.Text, dlgt.Caption, dlgt.Btns);
                            s.ShowQuestionDlg.ShowDialog = false;
                        }
                        else
                        {
                            if(s.Test != null)
                                _logger.Debug($"{s.Test.GetType().Name}: {s.Status}");
                            else
                                _logger.Debug($"Status: {s.Status}");

                            //runTextBox.ForeColor = s.StatusColor;
                            runTextBox.AppendText($"{DateTime.Now.ToString("hh:mm:ss")}: {s.Status}\r\n");

                            if (s.Exception != null)
                            {
                                _logger.Error(s.Exception, s.Test.GetType().Name);
                                runTextBox.AppendText(s.Exception.Message + "\r\n" + s.Exception.StackTrace + "\r\n");
                            }
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

