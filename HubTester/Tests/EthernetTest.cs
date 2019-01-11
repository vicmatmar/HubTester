using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubTests.Tests
{
    /// <summary>
    /// Simple test to make sure we can connect to the target
    /// Recommend to run it as the first test
    /// </summary>

    public class EthernetTest : TestBase
    {

        public EthernetTest(int timeout_sec)
        {
            Timeout_sec = timeout_sec;
        }

        int _timeout_sec = 60;
        public int Timeout_sec { get => _timeout_sec; set => _timeout_sec = value; }

        public override bool Setup()
        {
            // Do nothing...Don't connect

            return true;
        }

        public override bool Run()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();

            TestStatusTxt = $"Try to connect to hub for {Timeout_sec} sec";
            int connect_try = 0;
            while (stopWatch.Elapsed.TotalSeconds < Timeout_sec)
            {
                try
                {
                    
                    Connect();
                    TestStatusTxt = $"Connection successfull after {stopWatch.Elapsed.ToString(@"m\:ss")} sec.";
                    break;
                }
                catch(Exception ex)
                {
                    logger.Error(ex, $"Ethernet test connect try {++connect_try}");
                }
            }

            if (stopWatch.Elapsed.TotalSeconds >= Timeout_sec)
            {
                TestErrorTxt = "Timeout waiting for connection";
                return false;
            }
            return true;
        }
    }
}
