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

        public BuzzerTest(string ipAddress, string sshKeyFile, string userPrompt = null) : base(ipAddress, sshKeyFile)
        {
            this.userPrompt = userPrompt;
        }

        public override bool Run()
        {
            bool result = true;

            streamWriter.WriteLine("echo {0} > {1}/period", BUZZER_HZ, BUZZER_PATH);
            Thread.Sleep(50);

            streamWriter.WriteLine("echo {0} > {1}/duty_cycle", BUZZER_HZ / 2, BUZZER_PATH);
            Thread.Sleep(50);

            streamWriter.WriteLine("echo 1 > {0}/enable", BUZZER_PATH);
            Thread.Sleep(50);

            var dialogResult = MessageBox.Show(userPrompt, "Buzzer?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                TestStatusTxt = "Test Passed";
            }
            else if (dialogResult == DialogResult.No || dialogResult == DialogResult.Cancel)
            {
                TestStatusTxt = "Test Failed";
                result = false;
            }

            streamWriter.WriteLine("echo 0 > {0}/enable", BUZZER_PATH);
            Thread.Sleep(50);

            return result;
        }
    }
}
