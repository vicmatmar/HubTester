﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HubTests.Tests
{
    public class EmberTest : TestBase
    {
        private const string _device_to_join_EUIRegex = @"Device\ [0-9]:\ ([0-9a-fA-F]{16})\.([0-9])\ \((0x[0-9a-fA-F]{4})\)";
        private const string _hub_EUIRegex = @"\[\(>\)([0-9,A-F]{16})\]";
        readonly string _expected_device_to_join_EUI;
        const string _gateway_prompt = @"stratus_gateway>";

        public EmberTest(string device_to_join_eui) : base()
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

            this._expected_device_to_join_EUI = device_to_join_eui;
        }

        public override bool Setup()
        {
            bool result = base.Setup();

            if (!result)
                return result;

            string line = WriteCommand("");
            line = WriteCommand("");

            // We need to wait for these files to be created before we
            // stop the monitor
            // /config/activation_key
            // /data/run/.system
            string[] files = new string[]
            {
                @"/config/activation_key",
                @"/data/run/.system"
            };

            // Init a dictionary to keep track of found files
            var map = new Dictionary<string, bool>();
            foreach(var file in files)
                map.Add(file, false);

            var timeout_sec = 10;
            TestStatusTxt = $"Wait for hub init files {timeout_sec}s";
            var found_files = false;
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (stopwatch.Elapsed.TotalSeconds < timeout_sec)
            {
                // Go through the list of files...check whether they exit
                foreach(string file in files)
                {
                    if (!map[file])
                    {
                        line = WriteCommand($"ls {file}");
                        if (line == file)
                            map[file] = true;
                    }
                }

                found_files = true;
                foreach (string file in map.Keys)
                    found_files &= map[file];

                if (found_files)
                    break;

                Thread.Sleep(500);
            }
            if(!found_files)
            {
                TestErrorTxt = "Hub init files not found";
                return false;
            }


            TestStatusTxt = "Stop gateway";
            line = WriteCommand("monit stop stratus");

            TestStatusTxt = "Clearing Database";
            line = WriteCommand("rm -f /tmp/zigbee-ember.db");


            TestStatusTxt = "Start gateway";
            line = WriteCommand("cd /data/run/v*/zigbee");
            line = WriteCommand("export ZIGBEE_DEBUG=1");
            line = WriteCommand("export RPC_HOST=1338");
            line = WriteCommand("export DB_BASE_PATH=/tmp");
            line = WriteCommand("./stratus_gateway -p ttyO2", 20, "EMBER_NETWORK_UP");

            line = WriteGatewayCmd("", timeout_sec: 5);
            line = WriteGatewayCmd("");

            // Get hub EUI
            TestStatusTxt = "Get hub EUI";
            string info = WriteGatewayCmd("info");
            string hub_eui = Regex.Match(info, _hub_EUIRegex).Groups[1].Value;
            TestStatusTxt = $"Hub EUI String: {hub_eui}";
            HUB_EUI = hub_eui;

            return result;
        }

        string WriteGatewayCmd(string comand, int timeout_sec = 1, string prompt = _gateway_prompt, int cmd_delay_ms = 100)
        {
            return WriteCommand(comand, timeout_sec: timeout_sec, prompt: prompt, cmd_delay_ms: cmd_delay_ms);
        }

        public override bool Run()
        {
            bool testResult = false;

            string rexpeui = EUIToLittleEndian(_expected_device_to_join_EUI);

            string line = WriteGatewayCmd($"network form 12 0 0x2222");

            const int pjoin_access_time = 10;
            TestStatusTxt = $"Permit Join for {pjoin_access_time}s";
            line = WriteGatewayCmd($"network pjoin {pjoin_access_time}");

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Restart();

            int device_found_timeout = 60;
            TestStatusTxt = $"Waiting on device EUI {_expected_device_to_join_EUI} for {device_found_timeout}s";
            string devlist = "";
            while (stopWatch.Elapsed.TotalSeconds < device_found_timeout)
            {
                //double pjoin_etime = pjoin_access_time - stopWatch.Elapsed.TotalSeconds;
                //double timeout_time = device_found_timeout - stopWatch.Elapsed.TotalSeconds;
                //if (pjoin_etime > 0.0)
                //{
                //    string msg = $"PJoin Enabled: {pjoin_etime.ToString("F2")}. Timeout: {timeout_time.ToString("F2")}";
                //    logger.Debug(msg);
                //}
                //else
                //{
                //    string msg = $"PJoin Expired. Timeout: {timeout_time.ToString("F2")}";
                //    logger.Debug(msg);
                //}

                ReadToEnd();
                devlist = WriteGatewayCmd("custom listDevice");

                if (Regex.IsMatch(devlist, _device_to_join_EUIRegex))
                {
                    logger.Debug(devlist);
                    var matches = Regex.Matches(devlist, _device_to_join_EUIRegex);
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

                Thread.Sleep(500);
            }

            string devlist2 = WriteGatewayCmd("custom listDevice 1");
            logger.Debug($"DevList\r\n{devlist2}");

            WriteGatewayCmd($"network pjoin -1");
            line = WriteGatewayCmd("custom exit");

            if (testResult)
            {
                return true;
            }
            else
            {
                TestErrorTxt = $"Zigbee device EUI {_expected_device_to_join_EUI} not found. Devlist =\r\n {devlist}";
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
