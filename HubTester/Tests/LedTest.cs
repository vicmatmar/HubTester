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

        public LedTest() : base() { }

        public override bool Run()
        {
            TestStatusTxt = "Testing LEDs";
            bool result = false;

            WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "red"));
            WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "yellow"));
            WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "green"));

            DialogResult dialogResult = DialogResult.None;

            Task.Factory.StartNew(() =>
            {
                while (dialogResult == DialogResult.None)
                {
                    try
                    {
                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                        Thread.Sleep(500);

                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                        Thread.Sleep(500);
                    }
                    catch
                    {
                        break;
                    }
                }
            });

            //dialogResult = MessageBox.Show("Are LEDs flashing?", "LEDs?", MessageBoxButtons.YesNo);
            TestStatusQuestion = new ShowQuestionDlg("Are LEDs flashing?", "LEDs?", MessageBoxButtons.YesNo);
            dialogResult = TestStatus.ShowQuestionDig.DialogResult;
            result = dialogResult == DialogResult.Yes;

            // Leave leds on
            WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
            WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
            WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

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
