using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class LedTest : TestBase
    {

        public LedTest() : base() { }

        public override bool Run()
        {
            TestStatusTxt = "Testing LEDs";

            string line = WriteCommand("");
            WriteCommand("");

            WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "red"));
            WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "yellow"));
            WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "green"));

            DialogResult dialogResult = DialogResult.None;

            CancellationTokenSource cts = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() =>
            {
                while (dialogResult == DialogResult.None)
                {
                    if (cts.Token.IsCancellationRequested)
                        break;

                    try
                    {
                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "red"), cmd_delay_ms: 100);
                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"), cmd_delay_ms: 100);
                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "green"), cmd_delay_ms: 100);

                        Thread.Sleep(250);

                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "red"), cmd_delay_ms: 100);
                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"), cmd_delay_ms: 100);
                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "green"), cmd_delay_ms: 100);

                        Thread.Sleep(250);
                    }
                    catch
                    {
                        break;
                    }
                }
            }, cts.Token);

            //dialogResult = MessageBox.Show("Are LEDs flashing?", "LEDs?", MessageBoxButtons.YesNo);
            TestStatusQuestion = new ShowQuestionDlg("Are LEDs flashing?", "LEDs?", MessageBoxButtons.YesNo);
            dialogResult = TestStatus.ShowQuestionDlg.DialogResult;

            cts.Cancel();
            task.Wait();

            // Leave leds on
            WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "red"), timeout_sec: 2);
            WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"), timeout_sec: 2);
            WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, "green"), timeout_sec: 2);

            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                TestErrorTxt = "LEDs were not all flashing";
                return false;
            }
        }
    }
}
