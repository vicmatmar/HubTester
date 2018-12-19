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

        public EmberTest(string ipAddress, string sshKeyFile, string testDeviceEui) : base(ipAddress, sshKeyFile)
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

            string output = "";
            try
            {
                TestStatusTxt = "Stop gateway";
                streamWriter.WriteLine("monit stop stratus");
                output += streamReader.ReadLine();

                TestStatusTxt = "Clearing Database";
                streamWriter.WriteLine("rm -f /tmp/zigbee-ember.db");
                output += streamReader.ReadLine();

                //if(File.Exists())


                TestStatusTxt = "Starting Stratus Gateway";
                streamWriter.WriteLine("cd /data/run/v*/zigbee; ZIGBEE_DEBUG=1 RPC_HOST=1338 DB_BASE_PATH=/tmp ./stratus_gateway -p ttyO2");
                Thread.Sleep(250);

                string line = streamReader.ReadLine();
                output += line;
                while (!string.IsNullOrWhiteSpace(line))
                {
                    Thread.Sleep(500);
                    line = streamReader.ReadToEnd();
                    output += line;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public override bool Run()
        {

            bool testResult = false;

            string line = "";
            line = streamReader.ReadToEnd();
            while (!string.IsNullOrWhiteSpace(line))
            {
                Thread.Sleep(500);
                line += streamReader.ReadToEnd();
            }

            streamWriter.WriteLine($"network form 12 0 0x2222");
            Thread.Sleep(500);
            line = streamReader.ReadToEnd();

            const int pjoin_access_time = 30;
            streamWriter.WriteLine($"network pjoin {pjoin_access_time}");

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();

            Thread.Sleep(500);
            line = streamReader.ReadToEnd();


            string buffer = "";
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

                streamWriter.WriteLine("custom listDevice");
                Thread.Sleep(500);
                line = streamReader.ReadToEnd();
                buffer += line;

                while (!string.IsNullOrWhiteSpace(line))
                {
                    if (Regex.IsMatch(buffer, EUIRegex))
                    {
                        var matches = Regex.Matches(buffer, EUIRegex);

                        foreach (Match match in matches)
                        {
                            if (match.Groups[2].Value == testDeviceEui)
                            {
                                testResult = true;

                                streamWriter.WriteLine($"network pjoin -1");
                                Thread.Sleep(250);
                                line = streamReader.ReadToEnd();
                                buffer += line;
                            }

                            TestStatusTxt = string.Format("Leave {0}", match.Groups[2].Value);

                            streamWriter.WriteLine("zdo leave {0} 1 0", match.Groups[4].Value);
                            Thread.Sleep(250);
                            line = streamReader.ReadToEnd();
                            buffer += line;

                            streamWriter.WriteLine($"network pjoin -1");
                            Thread.Sleep(250);
                            line = streamReader.ReadToEnd();
                            buffer += line;

                            System.Diagnostics.Debug.WriteLine("=====================================");
                            System.Diagnostics.Debug.WriteLine(buffer);
                            System.Diagnostics.Debug.WriteLine("=====================================");
                        }

                        if (testResult)
                            break;
                    }

                    if (testResult)
                        break;

                    Thread.Sleep(1000);
                    line = streamReader.ReadToEnd();
                    buffer += line;
                }

                if (testResult)
                    break;
            }

            streamWriter.WriteLine("custom exit");
            Thread.Sleep(50);

            if (testResult)
            {
                TestStatusTxt = "Test Passed";
            }
            else
            {
                TestStatusTxt = "Test Failed";
            }

            return testResult;
        }

        public override bool TearDown()
        {
            bool tearDownResult = true;
            try
            {
                streamWriter.WriteLine("rm -f /tmp/zigbee-ember.db");
                Thread.Sleep(50);

                tearDownResult &= base.TearDown();
            }
            catch
            {
                tearDownResult = false;
            }

            return tearDownResult;
        }
    }
}
