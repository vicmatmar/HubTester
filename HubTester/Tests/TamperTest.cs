using System.Diagnostics;
using System.Threading;

namespace HubTests.Tests
{
    public class TamperTest : TestBase
    {
        private const long TAMPER_TIMEOUT = 30;
        
        public TamperTest() : base() { }

        public override bool Run()
        {
            bool result = false;
            string line = "";

            long seconds = 0;
            var stopWatch = new Stopwatch();

            // Make sure button is not stuck pressed
            logger.Trace("Detect button no stuck pressed");
            TestStatusTxt = "Detect tamper button is NOT pressed";
            stopWatch.Start();
            bool buttonPressed = true;
            while (seconds <= TAMPER_TIMEOUT && !result)
            {
                WriteLine("cat /sys/class/gpio/gpio44/value");
                Thread.Sleep(250);
                line = ReadLine();
                if (line == "1")
                {
                    buttonPressed = false;
                    break;
                }
                seconds = stopWatch.ElapsedMilliseconds / 1000;
            }
            stopWatch.Stop();

            if (buttonPressed)
            {
                TestStatus.Status = "Tamper button was found pressed";
                logger.Trace(TestStatus.Status);
                return false;
            }

            stopWatch.Start();
            TestStatusTxt = "Press and Hold Tamper/Button";
            logger.Trace("Detect button is pressed");
            while (seconds <= TAMPER_TIMEOUT && !result)
            {
                WriteLine("cat /sys/class/gpio/gpio44/value");
                Thread.Sleep(250);

                line = ReadLine();
                if (line == "0")
                {
                    buttonPressed = true;
                    result = true;
                }
                seconds = stopWatch.ElapsedMilliseconds / 1000;
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
