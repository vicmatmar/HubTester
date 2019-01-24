using System.Windows.Forms;

namespace HubTester.Tests
{
    public class BuzzerTest : TestBase
    {
        private const int BUZZER_HZ = 370370;
        private const string BUZZER_PATH = @"/sys/class/pwm/pwmchip0/pwm1";
    
        public BuzzerTest() : base()
        {
        }

        public override bool Run()
        {
            string rs = WriteCommand($"echo {BUZZER_HZ} > {BUZZER_PATH}/period");
            rs = WriteCommand($"echo {BUZZER_HZ/2} > {BUZZER_PATH}/duty_cycle");
            rs = WriteCommand($"echo 1 > {BUZZER_PATH}/enable");

            //var dialogResult = MessageBox.Show("Is Buzzer Active?", "Buzzer?", MessageBoxButtons.YesNo);
            TestStatusQuestion = new ShowQuestionDlg("Is Buzzer Active?", "Buzzer?", MessageBoxButtons.YesNo);
            var dialogResult = TestStatus.ShowQuestionDlg.DialogResult;
            while (TestStatus.ShowQuestionDlg.DialogResult == DialogResult.None) ;

            // Turn it off
            rs = WriteCommand($"echo 0 > {BUZZER_PATH}/enable");

            if (dialogResult == DialogResult.No || dialogResult == DialogResult.Cancel)
            {
                TestErrorTxt = "Buzzer was not active";
                return false;
            }

            return true;
        }
    }
}
