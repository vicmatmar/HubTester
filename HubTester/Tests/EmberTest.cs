using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HubTests.Tests
{
    public class EmberTest : TestBase
    {
        private const string EUIRegex = "(([0-9a-fA-F]{16})\\.([0-9])\\ \\((0x[0-9a-fA-F]{4})\\))";

        string testDeviceEui;

        public EmberTest(string testDeviceEui) : base()
        {
            StringBuilder builder = new StringBuilder();

            //var reverseEui = testDeviceEui.ToCharArray();
            //Array.Reverse(reverseEui);
            //testDeviceEui = new string(reverseEui);

            //for (var i = 0; i < testDeviceEui.Length; i = i + 2)
            //{
            //    var temp = testDeviceEui.Substring(i, 2);
            //    var reverseTemp = temp.ToCharArray();
            //    Array.Reverse(reverseTemp);
            //    builder.Append(new string(reverseTemp));
            //}

            //this.testDeviceEui = builder.ToString();

            this.testDeviceEui = testDeviceEui;
        }

        public override bool Setup()
        {
            bool result = base.Setup();

            if (!result)
                return result;

            TestStatusTxt = "Stop gateway";
            string line = WriteCommand("monit stop stratus");

            TestStatusTxt = "Clearing Database";
            line = WriteCommand("rm -f /tmp/zigbee-ember.db");


            TestStatusTxt = "Starting Stratus Gateway";
            line = WriteCommand("cd /data/run/v*/zigbee");
            line = WriteCommand("export ZIGBEE_DEBUG=1");
            line = WriteCommand("export RPC_HOST=1338");
            line = WriteCommand("export DB_BASE_PATH=/tmp");
            line = WriteCommand("./stratus_gateway -p ttyO2", 30, "EMBER_NETWORK_UP");

            return result;
        }

        public override bool Run()
        {
            string prompt = "stratus_gateway";
            bool testResult = false;

            WriteLine("");

            string line = WriteCommand($"network form 12 0 0x2222", prompt: prompt);

            const int pjoin_access_time = 30;
            line = WriteCommand($"network pjoin {pjoin_access_time}", prompt: prompt);

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            int device_found_timeout = 60;
            while (stopWatch.Elapsed.TotalSeconds < device_found_timeout)
            {
                double pjoin_etime = pjoin_access_time - stopWatch.Elapsed.TotalSeconds;
                double timeout_time = device_found_timeout - stopWatch.Elapsed.TotalSeconds;
                if (pjoin_etime > 0.0)
                {
                    TestStatusTxt = $"PJoin Enabled: {pjoin_etime.ToString("F2")}. Timeout: {timeout_time.ToString("F2")}";
                }
                else
                {
                    TestStatusTxt = $"PJoin Expired. Timeout: {timeout_time.ToString("F2")}";
                }

                line = WriteCommand("custom listDevice");
                while (!string.IsNullOrWhiteSpace(line))
                {
                    if (Regex.IsMatch(line, EUIRegex))
                    {
                        var matches = Regex.Matches(line, EUIRegex);

                        foreach (Match match in matches)
                        {
                            if (match.Groups[2].Value == testDeviceEui)
                            {
                                testResult = true;
                                WriteCommand($"network pjoin -1");
                            }

                            TestStatusTxt = string.Format("Leave {0}", match.Groups[2].Value);

                            WriteCommand($"zdo leave {match.Groups[4].Value} 1 0");

                            WriteCommand($"network pjoin -1");

                        }

                        if (testResult)
                            break;
                    }

                    if (testResult)
                        break;
                }

                if (testResult)
                    break;
            }

            WriteCommand("custom exit");

            if (testResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool TearDown()
        {
            bool tearDownResult = true;

            WriteCommand("rm -f /tmp/zigbee-ember.db");

            tearDownResult &= base.TearDown();

            return tearDownResult;
        }
    }
}
