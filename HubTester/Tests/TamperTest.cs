using System.Diagnostics;
using System.Threading;

namespace HubTests.Tests
{
    public class TamperTest : TestBase
    {
        private const long TAMPER_TIMEOUT = 5;
        
        public TamperTest(string ipAddress, string sshKeyFile) : base(ipAddress, sshKeyFile) { }

        public override bool Run()
        {
            bool result = false;
            string line = "";

            long seconds = 0;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            TestStatusTxt = "Press and Hold Tamper/Button";
            while (seconds <= TAMPER_TIMEOUT && !result)
            {
                seconds = stopWatch.ElapsedMilliseconds / 1000;

                streamWriter.WriteLine("cat /sys/class/gpio/gpio44/value");
                Thread.Sleep(50);

                line = streamReader.ReadLine();

                if (line == "0")
                {
                    result = true;
                }
            }
            stopWatch.Stop();

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
