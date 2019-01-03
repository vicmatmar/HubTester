using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class BuzzerTest : TestBase
    {
        private const int BUZZER_HZ = 370370;
        private const string BUZZER_PATH = @"/sys/class/pwm/pwmchip0/pwm1";
    
        private string userPrompt;

        public BuzzerTest(string userPrompt = null) : base()
        {
            this.userPrompt = userPrompt;
        }

        public override bool Run()
        {
            bool result = true;

            string rs = WriteCommand($"echo {BUZZER_HZ} > {BUZZER_PATH}/period");
            rs = WriteCommand($"echo {BUZZER_HZ/2} > {BUZZER_PATH}/duty_cycle");
            rs = WriteCommand($"echo 1 > {BUZZER_PATH}/enable");

            //var dialogResult = MessageBox.Show(userPrompt, "Buzzer?", MessageBoxButtons.YesNo);
            TestStatusQuestion = new ShowQuestionDlg(userPrompt, "Buzzer?", MessageBoxButtons.YesNo);
            var dialogResult = TestStatus.ShowQuestionDig.DialogResult;

            // Trun it off
            rs = WriteCommand($"echo 0 > {BUZZER_PATH}/enable");

            result = dialogResult == DialogResult.Yes;
            if (result)
            {
                TestStatusTxt = "Test Passed";
            }
            else if (dialogResult == DialogResult.No || dialogResult == DialogResult.Cancel)
            {
                TestStatusTxt = "Test Failed";
                result = false;
            }


            return result;
        }
    }
}
