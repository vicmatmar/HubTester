using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
            if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Canceled"; return false; }

            bool connected = false;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Restart();

            TestStatusTxt = $"Try to connect to hub for {Timeout_sec}s";
            int connect_try = 0;
            while (stopWatch.Elapsed.TotalSeconds < Timeout_sec)
            {
                if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Canceled"; return false; }
                try
                {
                    
                    Connect();
                    connected = true;
                    TestStatusTxt = $"Connection successful after {stopWatch.Elapsed.ToString(@"m\:ss")}";
                    break;
                }
                catch(Exception ex)
                {
                    logger.Error(ex, $"Ethernet test connect try {++connect_try}");
                    Thread.Sleep(1000);
                }
            }

            if (!connected)
            {
                TestErrorTxt = "Timeout waiting for connection";
                return false;
            }
            else
                return true;
        }
    }
}
