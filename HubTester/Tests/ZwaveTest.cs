using System.Threading;

namespace HubTests.Tests
{
    public class ZwaveTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;
        public ZwaveTest(string ipAddress, string sshKeyFile) : base(ipAddress, sshKeyFile) { }

        public override bool Run()
        {
            bool result = false;

            try
            {
                TestStatusTxt = "Running Zwave Test";
                streamWriter.WriteLine("/data/support/zwave_nvram -g 0");
                Thread.Sleep(50);

                int retries = 0;
                string line = "";
                line = streamReader.ReadToEnd();
                while (line != null || retries <= RETRY_TIMEOUT)
                {
                    if (retries > RETRY_TIMEOUT)
                    {
                        result = false;
                        break;
                    }
                    else if (line.Contains("0x00:  ff"))
                    {
                        result = true;
                        break;
                    }

                    retries++;
                    Thread.Sleep(500);
                    line = streamReader.ReadToEnd();
                }

                if (result)
                {
                    TestStatusTxt = "Test Passed";
                }
                else
                {
                    TestStatusTxt = "Test Failed";
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}
