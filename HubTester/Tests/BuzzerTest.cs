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

            WriteLine("echo {0} > {1}/period", BUZZER_HZ, BUZZER_PATH);
            rs = WaitForPrompt();
            WriteLine("echo {0} > {1}/duty_cycle", BUZZER_HZ / 2, BUZZER_PATH);
            rs = WaitForPrompt();
            WriteLine("echo 1 > {0}/enable", BUZZER_PATH);
            rs = WaitForPrompt();

            //var dialogResult = MessageBox.Show(userPrompt, "Buzzer?", MessageBoxButtons.YesNo);
            TestStatusQuestion = new ShowQuestionDlg(userPrompt, "Buzzer?", MessageBoxButtons.YesNo);
            var dialogResult = TestStatus.ShowQuestionDig.DialogResult;

            // I'm not sure why I have to write this twice to take effect
            // It started when I moved to using teststatus ShowQuestionDigbut not sure why
            WriteLine("echo 0 > {0}/enable", BUZZER_PATH);
            rs = WaitForPrompt();
            WriteLine("echo 0 > {0}/enable", BUZZER_PATH);
            rs = WaitForPrompt();

            if (dialogResult == DialogResult.Yes)
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
