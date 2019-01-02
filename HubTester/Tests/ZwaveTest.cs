using System.Text.RegularExpressions;
using System.Threading;

namespace HubTests.Tests
{
    public class ZwaveTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;
        public ZwaveTest() : base() { }

        public override bool Setup()
        {
            if (!base.Setup())
                return false;

            WriteLine("ls /data/support/zwave_test");

            Regex regx = new Regex(@"/data/support/zwave_test\r\n.*/data/support/zwave_test");
            string l = ReadUntil(regx, 3);


            return true;


        }
        public override bool Run()
        {
            bool result = false;

            try
            {
                TestStatusTxt = "Running Zwave Test";
                WriteLine("/data/support/zwave_nvram -g 0");
                Thread.Sleep(50);

                int retries = 0;
                string line = "";
                line = ReadToEnd();
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
                    line = ReadToEnd();
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
