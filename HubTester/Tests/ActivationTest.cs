using System.Threading;

namespace HubTests.Tests
{
    public class ActivationTest : TestBase
    {
        private const string REPORT_KEY_COMMAND = @"if [ -f /config/activation_key ]; then cat /config/activation_key; echo; else echo 'No Activation Key Found'; fi";
        public string ActivationCode { get; private set; }

        public ActivationTest(string ipAddress, string sshKeyFile) : base(ipAddress, sshKeyFile) { }

        public override bool Run()
        {
            bool result = false;
            string line = "";

            // Read Activation Key from board
            streamWriter.WriteLine(REPORT_KEY_COMMAND);
            Thread.Sleep(500);

            line = streamReader.ReadLine();
            line = streamReader.ReadLine();
            line = streamReader.ReadLine();
            line = streamReader.ReadLine();

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
