using System.Threading;

namespace HubTester.Tests
{
    public class BluetoothTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;
        public BluetoothTest() : base() { }

        public override bool Run()
        {
            TestStatusTxt = "Running Bluetooth Test";
            bool result = false;

            try
            {
                WriteLine("python /data/support/bluetooth/bluetooth.py");
                Thread.Sleep(1000);

                int retries = 0;
                string line = ReadToEnd();
                while (retries <= RETRY_TIMEOUT)
                {
                    if (line != null && line.Contains("Device initialized and ready"))
                    {
                        result = true;
                        break;
                    }

                    retries++;
                    line = ReadToEnd();
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
