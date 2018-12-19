using System.Threading;

namespace HubTests.Tests
{
    public class BluetoothTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;
        public BluetoothTest(string ipAddress, string sshKeyFile) : base(ipAddress, sshKeyFile) { }

        public override bool Run()
        {
            TestStatusTxt = "Running Bluetooth Test";
            bool result = false;

            try
            {
                streamWriter.WriteLine("python /data/support/bluetooth/bluetooth.py");
                Thread.Sleep(1000);

                int retries = 0;
                string line = streamReader.ReadToEnd();
                while (retries <= RETRY_TIMEOUT)
                {
                    if (line != null && line.Contains("Device initialized and ready"))
                    {
                        result = true;
                        break;
                    }

                    retries++;
                    line = streamReader.ReadToEnd();
                    Thread.Sleep(500);
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
    }
}
