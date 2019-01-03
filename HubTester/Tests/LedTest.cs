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

            WriteLine("echo none > " + string.Format(LED_TRIGGER_PATH, "red"));
            WriteLine("echo none > " + string.Format(LED_TRIGGER_PATH, "yellow"));
            WriteLine("echo none > " + string.Format(LED_TRIGGER_PATH, "green"));
            Thread.Sleep(50);

            DialogResult dialogResult = DialogResult.None;

            Task.Run(() =>
            {
                while (dialogResult == DialogResult.None)
                {
                    try
                    {
                        WriteLine("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                        WriteLine("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                        WriteLine("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                        Thread.Sleep(1000);

                        WriteLine("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                        WriteLine("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                        WriteLine("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                        Thread.Sleep(1000);
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
