using System.Text.RegularExpressions;

namespace HubTester.Tests
{
    public class TamperTest : TestBase
    {
        private const long TAMPER_TIMEOUT = 30;

        public TamperTest() : base() { }

        public override bool Run()
        {
            string rs = "";
            Regex regx = new Regex(@"\r\n([0-1])\r\n");
            bool buttonPressed = true;

            // Make sure button is not stuck pressed
            TestStatusTxt = "Detect tamper button is NOT pressed";
            while (true)
            {
                if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Run Canceled"; return false; }

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

            TestStatusTxt = "Press Tamper Button";
            while (true)
            {
                if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Run Canceled"; return false; }

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
            while (true)
            {
                if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Canceled"; return false; }

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

            return true;
        }
    }
}
