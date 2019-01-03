using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

namespace HubTests.Tests
{
    public class TamperTest : TestBase
    {
        private const long TAMPER_TIMEOUT = 30;

        public TamperTest() : base() { }

        public override bool Run()
        {
            string rs = "";
            var stopWatch = new Stopwatch();
            Regex regx = new Regex(@"\r\n([0-1])\r\n");
            bool buttonPressed = true;

            // Make sure button is not stuck pressed
            TestStatusTxt = "Detect tamper button is NOT pressed";
            stopWatch.Restart();
            while (stopWatch.Elapsed.TotalSeconds <= TAMPER_TIMEOUT)
            {
                rs = WriteCommand("cat /sys/class/gpio/gpio44/value");
                if (rs == "1")
                {
                    buttonPressed = false;
                    break;
                }
            }
            if (buttonPressed)
            {
                TestStatus.Status = "Tamper button was found pressed";
                return false;
            }

            stopWatch.Restart();
            TestStatusTxt = "Press Tamper/Button";
            while (stopWatch.Elapsed.TotalSeconds <= TAMPER_TIMEOUT)
            {
                rs = WriteCommand("cat /sys/class/gpio/gpio44/value");
                if (rs == "0")
                {
                    buttonPressed = true;
                    break;
                }
            }
            if (!buttonPressed)
            {
                TestStatus.Status = "Unable to detect Tamper button pressed";
                return false;
            }

            TestStatusTxt = "Release the Tamper/Button";
            stopWatch.Restart();
            while (stopWatch.Elapsed.TotalSeconds <= TAMPER_TIMEOUT)
            {
                rs = WriteCommand("cat /sys/class/gpio/gpio44/value");
                if (rs == "1")
                {
                    buttonPressed = false;
                    break;
                }
            }
            if (buttonPressed)
            {
                TestStatus.Status = "Tamper button was found pressed";
                logger.Trace(TestStatus.Status);
                return false;
            }

            TestStatusTxt = "Test Passed";

            return true;
        }
    }
}
