using System.Threading;

namespace HubTester.Tests
{
    public class ActivationTest : TestBase
    {
        private const string REPORT_KEY_COMMAND = @"if [ -f /config/activation_key ]; then cat /config/activation_key; echo; else echo 'No Activation Key Found'; fi";
        public string ActivationCode { get; private set; }

        public ActivationTest() : base() { }

        public override bool Run()
        {
            bool result = false;
            string line = "";

            // Read Activation Key from board
            WriteLine(REPORT_KEY_COMMAND);
            Thread.Sleep(500);

            line = ReadLine();
            line = ReadLine();
            line = ReadLine();
            line = ReadLine();

            if (line != "No Activation Key Found")
            {
                ActivationCode = line;
                result = true;
            }

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
