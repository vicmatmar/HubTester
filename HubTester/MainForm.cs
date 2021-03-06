﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.Entity.Validation;
using HubTester.Tests;

namespace HubTester
{
    public partial class MainForm : Form
    {
        static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        IProgress<TestStatus> _progress;
        Stopwatch sequenceStopWatch = new Stopwatch();

        string _ipaddress = "192.168.10.2";
        protected string Ipaddress { get => _ipaddress; }

        TestSequence testSequence = new TestSequence();
        IProgress<string> _testSequenceProgress;

        int _testIndex = 0;
        protected int TestIndex { get => _testIndex; set => _testIndex = value; }

        ITest blinkLed;
        Task blinkLedTask;

        string nowTimeStamp { get => DateTime.Now.ToString("hh:mm:ss"); } 

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _testSequenceProgress = new Progress<string>(s =>
            {
                switch (s)
                {
                    case "IsRunning":
                        OnTestSequenceRunningChanged();
                        break;
                    case "HUB_EUI":
                        runTextBox.AppendText($"{nowTimeStamp}: Test Sequence HUB EUI: {testSequence.HUB_EUI}\r\n");
                        break;
                    case "HUB_MAC_ADDR":
                        runTextBox.AppendText($"{nowTimeStamp}: Test Sequence HUB MAC ADDR: {testSequence.HUB_MAC_ADDR}\r\n");
                        break;
                    default:
                        break;
                }
            });

            testSequence.PropertyChanged += TestSequence_PropertyChanged;
        }

        void OnTestSequenceRunningChanged()
        {
            bool isRunning = testSequence.IsRunning;
            if (isRunning)
            {
                cancelButton.Enabled = true;
                runButton.Enabled = false;

                if (blinkLedTask != null)
                {
                    blinkLed.Cancel();
                    while (!blinkLedTask.IsCompleted)
                        Thread.Sleep(100);
                }
            }
            else
            {
                if (TestIndex >= testSequence.Count)
                {
                    runButton.Text = "&Run";
                    cancelButton.Enabled = false;
                }
                else
                {
                    runButton.Text = "&Rerun";
                    cancelButton.Enabled = true;
                }
                runButton.Enabled = true;

                //blinkLed = new LedBlinker("green");
                //blinkLedTask = Task.Factory.StartNew(() => blinkLed.Run());
            }
        }

        private void TestSequence_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _testSequenceProgress.Report(e.PropertyName);
        }

        void AddTest(ITest test)
        {
            test.PropertyChanged -= Test_PropertyChanged;
            test.PropertyChanged += Test_PropertyChanged;
            test.TestSequence = testSequence;
            testSequence.Add(test);

        }

        private void LoadTests()
        {

            FileStream fs = new FileStream("tests.json", FileMode.Open);
            StreamWriter sw = new StreamWriter(fs);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(testSequence.GetType());
            testSequence = (TestSequence)ser.ReadObject(sw.BaseStream);
            testSequence.Tests.ForEach(tc => {
                tc.PropertyChanged += Test_PropertyChanged;
                tc.TestSequence = testSequence;
            });
            testSequence.PropertyChanged += TestSequence_PropertyChanged;

            //testSequence.Clear();
            ////AddTest(new EthernetTest(120));
            //testSequence.HUB_EUI = "000D6F000B299253";
            //testSequence.HUB_MAC_ADDR = "00:00:00:00:00:01";
            //AddTest(new ActivationTest());


            //testSequence.Clear();
            //AddTest(new EthernetTest(120));
            //AddTest(new BuzzerTest());
            //AddTest(new EmberTest(Properties.Settings.Default.TestEui));
            //AddTest(new LedTest());
            //AddTest(new TamperTest());
            //AddTest(new UsbTest());
            //AddTest(new ZwaveTest());
            //AddTest(new TamperTest());

            //FileStream fs = new FileStream("tests.json", FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs);
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(testSequence.GetType());
            //ser.WriteObject(sw.BaseStream, testSequence);

            //AddTest(new Shutdown());
            //AddTest(new BluetoothTest());
            // Generate next MAC address and write to board
            //tests.Add(new MacTest(IpAddress, RsaFile, StartBlock, EndBlock));
            // Generate Activation Key and read from board
            //tests.Add(new ActivationTest(IpAddress, RsaFile));
            // Battery Test can cause the hub to reboot so make it final test
            //tests.Add(new BatteryTest(IpAddress, RsaFile));
            // Print Final Label
            //tests.Add(new PrintLabel(HubLabelPrinterAddress, ActivationCodePrinterAddress));

        }

        private void RunTests(IProgress<TestStatus> progress)
        {
            _progress = progress;

            if (TestIndex == 0)
                sequenceStopWatch.Restart();

            Stopwatch testStopWatch = new Stopwatch();

            //if (!TestsLoaded)
            TestStatus ts = new TestStatus(null, "Loading Tests");
            progress.Report(ts);

            // Tests have already ran and passed
            // Restart Testing from beginning
            if (TestIndex >= testSequence.Count)
            {
                TestIndex = 0;
            }

            while (TestIndex < testSequence.Count)
            {
                testStopWatch.Restart();

                testSequence.IsRunning = true;

                bool setupPassed = false;
                bool runPassed = false;
                bool tearDownPassed = false;

                ITest test = testSequence.Tests[TestIndex];

                _logger.Debug($"Run Test Index {TestIndex} of {testSequence.Count}: {test.GetType().Name}");

                ts = new TestStatus(test, $"{test.GetType().Name} ({TestIndex + 1}/{testSequence.Count})");
                progress.Report(ts);

                try
                {
                    setupPassed = test.Setup();

                    if (!setupPassed)
                    {
                        string msg = " Setup " + (test.IsCancellationRequested ? "Canceled" : "Failed");
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
                if (setupPassed && !test.IsCancellationRequested)
                {
                    try
                    {
                        runPassed = test.Run();
                        if (!runPassed)
                        {
                            string msg = " Run " + (test.IsCancellationRequested ? "Canceled" : "Failed");
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

            if (TestIndex >= testSequence.Count || testSequence.Tests[TestIndex].IsCancellationRequested)
            {
                sequenceStopWatch.Stop();
                string etimestr = $" ({sequenceStopWatch.Elapsed.ToString(@"m\:ss")})";

                ts = new TestStatus
                {
                    PropertyName = TestStatusPropertyNames.Status,
                };

                if (TestIndex >= testSequence.Count)
                    ts.Status = $"All Tests Passed";
                else if (testSequence.Tests[TestIndex].IsCancellationRequested)
                    ts.Status = $"Test sequence canceled";

                ts.Status += etimestr;
                progress.Report(ts);
            }
            testSequence.IsRunning = false;
        }

        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITest test = (ITest)sender;
            _progress.Report(test.TestStatus);
        }

        private async void RunButton_Click(object sender, EventArgs e)
        {
            _logger.Debug("Run button clicked");

            runButton.Enabled = false;
            cancelButton.Enabled = true;

            if (TestIndex >= testSequence.Count)
                TestIndex = 0;

            if (TestIndex == 0)
            {
                runTextBox.Clear();
                LoadTests();
            }

            var progress = new Progress<TestStatus>(s => OnTestCaseStatusChanged(s));
            await Task.Run(() => RunTests(progress));

            if (TestIndex >= testSequence.Count)
            {
                runButton.Text = "&Run";
                runButton.Enabled = true;
                cancelButton.Enabled = false;
            }

        }

        void OnTestCaseStatusChanged(TestStatus s)
        {
            if (Disposing)
                return;

            if (s.Test != null)
                _logger.Debug($"{s.Test.GetType().Name}: {s.Status}");
            else
                _logger.Debug($"Status: {s.Status}");

            switch (s.PropertyName)
            {
                case TestStatusPropertyNames.Status:
                    runTextBox.AppendText($"{nowTimeStamp}: {s.Status}\r\n");
                    break;
                case TestStatusPropertyNames.ErrorMsg:
                    runTextBox.AppendText($"{nowTimeStamp}: {s.ErrorMsg}\r\n");
                    break;
                case TestStatusPropertyNames.Exception:
                    _logger.Error(s.Exception, s.Test.GetType().Name);

                    runTextBox.AppendText($"{nowTimeStamp}: {s.Status}\r\n");
                    runTextBox.AppendText($"{s.Exception.Message}\r\n\r\n");
                    runTextBox.AppendText($"{s.Exception.StackTrace}\r\n");

                    if (s.Exception.InnerException.InnerException != null)
                    {
                        runTextBox.AppendText($"{s.Exception.InnerException.InnerException.Message}\r\n");
                    }

                    if (s.Exception is DbEntityValidationException)
                    {
                        DbEntityValidationException ex = (DbEntityValidationException)s.Exception;
                        foreach(var err in ex.EntityValidationErrors)
                        {
                            foreach(var e in err.ValidationErrors)
                                runTextBox.AppendText($"{e.ErrorMessage}\r\n");
                        }
                    }

                    break;
                case TestStatusPropertyNames.ShowQuestionDlg:
                    ShowQuestionDlg dlgt = s.ShowQuestionDlg;
                    _logger.Debug($"{s.Test.GetType().Name} show dialog: {dlgt.Text}, {dlgt.Caption}, {dlgt.Btns.ToString()}");
                    dlgt.DialogResult = MessageBoxEx.Show(this, dlgt.Text, dlgt.Caption, dlgt.Btns);

                    break;
                case TestStatusPropertyNames.HUB_EUI:
                    testSequence.HUB_EUI = s.Status;
                    break;
                case TestStatusPropertyNames.HUB_MAC:
                    testSequence.HUB_MAC_ADDR = s.Status;
                    break;
                default:
                    runTextBox.AppendText($"{nowTimeStamp}: Unhanded PropertyName\r\n");
                    break;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            foreach (var test in testSequence.Tests)
                test.Cancel();

            if (!testSequence.IsRunning)
            {
                TestIndex = 0;
                runButton.Text = "&Run";
                runButton.Enabled = true;
                cancelButton.Enabled = false;
            }

        }
    }
}

