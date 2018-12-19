using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class BatteryTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;

        public BatteryTest(string ipAddress, string sshKeyFile) : base(ipAddress, sshKeyFile) { }

        public override bool Run()
        {
            TestStatusTxt = "Running Battery Test";
            bool result = false;

            try
            {
                bool InitialConnectionResult = TryConnection();

                if (InitialConnectionResult)
                {
                    MessageBox.Show("Remove AC power, ensure battery power is on.");

                    result = TryConnection();
                }
            }
            catch
            {
                result = false;
            }

            if (result)
            {
                TestStatusTxt = "Test Passed";
            }
            else
            {
                TestStatusTxt = "Test Failed";
            }

            return result;
        }

        private bool TryConnection()
        {
            bool result = true;
            int retries = 0;
            string line = "";

            streamReader.ReadToEnd();
            streamWriter.WriteLine("spud");
            Thread.Sleep(500);

            line = streamReader.ReadToEnd();
            while (retries < RETRY_TIMEOUT)
            {
                if (line.Contains("Yes, this is spud."))
                {
                    break;
                }

                retries++;
                line = streamReader.ReadToEnd();
                Thread.Sleep(500);
            }

            if (retries >= RETRY_TIMEOUT)
            {
                result = false;
            }

            return result;
        }
    }
}
