using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HubTests.Tests
{
    public class EmberTest : TestBase
    {
        private const string _eUIRegex = @"Device\ [0-9]:\ ([0-9a-fA-F]{16})\.([0-9])\ \((0x[0-9a-fA-F]{4})\)";
        string _testDeviceEui;
        const string _gateway_prompt = @"stratus_gateway>";

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

            this._testDeviceEui = testDeviceEui;
        }

        public override bool Setup()
        {
            bool result = base.Setup();

            if (!result)
                return result;

            string line = WriteCommand("");
            line = WriteCommand("");

            TestStatusTxt = "Stop gateway";
            line = WriteCommand("monit stop stratus");

            TestStatusTxt = "Clearing Database";
            line = WriteCommand("rm -f /tmp/zigbee-ember.db");


            TestStatusTxt = "Starting Stratus Gateway";
            line = WriteCommand("cd /data/run/v*/zigbee");
            line = WriteCommand("export ZIGBEE_DEBUG=1");
            line = WriteCommand("export RPC_HOST=1338");
            line = WriteCommand("export DB_BASE_PATH=/tmp");
            line = WriteCommand("./stratus_gateway -p ttyO2", 15, "EMBER_NETWORK_UP");

            line = WriteGatewayCmd("", timeout_sec: 5);
            line = WriteGatewayCmd("");

            return result;
        }

        string WriteGatewayCmd(string comand, int timeout_sec = 1, string prompt = _gateway_prompt, int cmd_delay_ms = 100)
        {
            return WriteCommand(comand, timeout_sec: timeout_sec, prompt: prompt, cmd_delay_ms: cmd_delay_ms);
        }

        public override bool Run()
        {
            bool testResult = false;

            string rexpeui = EUIToLittleEndian(_testDeviceEui);

            string line = WriteGatewayCmd($"network form 12 0 0x2222");

            const int pjoin_access_time = 10;
            TestStatusTxt = $"Permit Join for {pjoin_access_time} sec";
            line = WriteGatewayCmd($"network pjoin {pjoin_access_time}");

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Restart();

            int device_found_timeout = 60;
            while (stopWatch.Elapsed.TotalSeconds < device_found_timeout)
            {
                double pjoin_etime = pjoin_access_time - stopWatch.Elapsed.TotalSeconds;
                double timeout_time = device_found_timeout - stopWatch.Elapsed.TotalSeconds;
                if (pjoin_etime > 0.0)
                {
                    string msg = $"PJoin Enabled: {pjoin_etime.ToString("F2")}. Timeout: {timeout_time.ToString("F2")}";
                    logger.Debug(msg);
                }
                else
                {
                    string msg = $"PJoin Expired. Timeout: {timeout_time.ToString("F2")}";
                    logger.Debug(msg);
                }

                line = WriteGatewayCmd("custom listDevice");
                if (Regex.IsMatch(line, _eUIRegex))
                {
                    logger.Debug(line);
                    var matches = Regex.Matches(line, _eUIRegex);
                    foreach (Match match in matches)
                    {
                        if (match.Groups[1].Value == rexpeui)
                        {
                            testResult = true;
                        }
                        logger.Debug($"EUI:{EUIToLittleEndian(match.Groups[1].Value)}");

                        // Device stays on the custom listDevice list even after it leaves
                        WriteGatewayCmd($"zdo leave {match.Groups[3].Value} 1 0");
                    }

                    if (testResult)
                    {
                        WriteGatewayCmd($"network pjoin -1");
                        break;
                    }
                }
                if (testResult)
                    break;
            }

            WriteGatewayCmd($"network pjoin -1");
            line = WriteGatewayCmd("custom exit");

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

            WriteLine("");
            WriteLine("");
            string rs = ReadToEnd();
            if (rs.Contains(_gateway_prompt))
            {
                WriteGatewayCmd("cu exit");
                WriteLine("");
                WriteLine("");
                rs = ReadToEnd();
            }

            WriteCommand("rm -f /tmp/zigbee-ember.db");

            tearDownResult &= base.TearDown();

            return tearDownResult;
        }
    }
}
