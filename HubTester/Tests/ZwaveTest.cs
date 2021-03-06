﻿
namespace HubTester.Tests
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

            // Check whether we need to copy supporting test until
            string remote_path = "/data/support/zwave_nvram";

            //bool need_to_copy = false;
            //WriteLine($"ls {remote_path}");
            //string t = $@"{remote_path}\r\n.*{remote_path}";
            //Regex regx = new Regex($@"{remote_path}\r\n.*{remote_path}");
            //try { ReadUntil(regx, 1); }
            //catch (ReadUntilTimeoutException) { need_to_copy = true; }

            // Copy test util
            ScpUpload("HubFiles/zwave_nvram", $"{remote_path}");
            WriteCommand($"chmod +x {remote_path}");

            return true;


        }
        public override bool Run()
        {
            TestStatusTxt = "Running Zwave Test";
            string rs = WriteCommand("/data/support/zwave_nvram -g 0", 5);
            bool result = rs == "Zwave NVRAM data:\r\n0x00:  ff";

            return result;
        }
    }
}
