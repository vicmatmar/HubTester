using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HubTests.Tests;

namespace HubTester
{
    public partial class MainForm : Form
    {

        IProgress<TestStatus> _progress;

        string _ipaddress = "192.168.10.2";
        protected string Ipaddress { get => _ipaddress;  }

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

        private void LoadTests()
        {

            Tests.Add(new LedTest(Ipaddress));
            //tests.Add(new TamperTest(IpAddress, RsaFile));
            //tests.Add(new UsbTest(IpAddress, RsaFile, "Insert USB to first USB Slot"));
            //tests.Add(new UsbTest(IpAddress, RsaFile, "Insert USB to second USB Slot"));
            //tests.Add(new BuzzerTest(IpAddress, RsaFile, "Is Buzzer Active?"));
            //tests.Add(new BluetoothTest(IpAddress, RsaFile));
            //tests.Add(new ZwaveTest(IpAddress, RsaFile));

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
                test.PropertyChanged += Test_PropertyChanged;

                testPassed &= test.Setup();

                // If setup fails, no reason to run test
                if (testPassed)
                {
                    testPassed &= test.Run();
                }

                testPassed &= test.TearDown();

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
                //TestStatus = "All tests passed successfully";
            }
        }

        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ITest test = (ITest)sender;
            _progress.Report(test.TestStatus);
        }

        private async void RunButton_Click(object sender, EventArgs e)
        {
            RunButton.Enabled = false;

            var progress =
                new Progress<TestStatus>(s =>
                    {
                        statusLabel.Text = s.Status;
                        if (s.Exception != null)
                            errorTextBox.Text = s.Exception.Message + "\r\n" + s.Exception.StackTrace;
                    }
                );

            await Task.Run(() => RunTests(progress));

            RunButton.Enabled = true;

        }
    }
}
