using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class LedTest : TestBase
    {
        private const string LED_TRIGGER_PATH = @"/sys/class/leds/{0}/trigger";
        private const string LED_BRIGHTNESS_PATH = @"/sys/class/leds/{0}/brightness";

        public LedTest(string ipAddress, string sshKeyFile) : base(ipAddress, sshKeyFile) { }
        public LedTest(string ipAddress) : base(ipAddress) { }

        public override bool Run()
        {
            TestStatusTxt = "Testing LEDs";
            bool result = false;

            streamWriter.WriteLine("echo none > " + string.Format(LED_TRIGGER_PATH, "red"));
            streamWriter.WriteLine("echo none > " + string.Format(LED_TRIGGER_PATH, "yellow"));
            streamWriter.WriteLine("echo none > " + string.Format(LED_TRIGGER_PATH, "green"));
            Thread.Sleep(50);

            DialogResult dialogResult = DialogResult.None;

            Task.Run(() =>
            {
                while (dialogResult == DialogResult.None)
                {
                    try
                    {
                        streamWriter.WriteLine("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                        streamWriter.WriteLine("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                        streamWriter.WriteLine("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                        Thread.Sleep(1000);

                        streamWriter.WriteLine("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                        streamWriter.WriteLine("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                        streamWriter.WriteLine("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                        Thread.Sleep(1000);
                    }
                    catch
                    {
                        break;
                    }
                }
            });

            dialogResult = MessageBox.Show("Are LEDs flashing?", "LEDs?", MessageBoxButtons.YesNo);
            result = dialogResult == DialogResult.Yes;

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
