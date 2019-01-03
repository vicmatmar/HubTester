using HubTest;
using System.Text.RegularExpressions;
using System.Threading;

namespace HubTests.Tests
{
    public class ZwaveTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;
        public ZwaveTest() : base() { }

        public override bool Setup()
        {
            // Call base setup to establish connection, etc
            if (!base.Setup())
                return false;

            // Check whether we need to copy supporting test util
            string remote_path = "/data/support/zwave_nvram";

            //bool need_to_copy = false;
            //WriteLine($"ls {remote_path}");
            //string t = $@"{remote_path}\r\n.*{remote_path}";
            //Regex regx = new Regex($@"{remote_path}\r\n.*{remote_path}");
            //try { ReadUntil(regx, 1); }
            //catch (ReadUntilTimeoutException) { need_to_copy = true; }

            // Copy test util
            ScpUpload("HubFiles/zwave_nvram", $"{remote_path}");
            WriteLine($"chmod +x {remote_path}");

            return true;


        }
        public override bool Run()
        {
            TestStatusTxt = "Running Zwave Test";
            WriteLine("/data/support/zwave_nvram -g 0");
            string rs = ReadUntil(new Regex(Regex.Escape("0x00:  ff")), 3);

            TestStatusTxt = "Test Passed";

            return true;
        }
    }
}
