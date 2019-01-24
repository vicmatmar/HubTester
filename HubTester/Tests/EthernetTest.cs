using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace HubTester.Tests
{
    /// <summary>
    /// Simple test to make sure we can connect to the target
    /// Recommend to run it as the first test
    /// </summary>

    public class EthernetTest : TestBase
    {
        public EthernetTest():base()
        {
            Timeout_sec = 60;
        }

        public EthernetTest(int timeout_sec):base()
        {
            Timeout_sec = timeout_sec;
        }

        int _timeout_sec = 60;
        [DataMember]
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
            int sucesfull_connections = 0;
            while (stopWatch.Elapsed.TotalSeconds < Timeout_sec)
            {
                if (CancelToken.IsCancellationRequested) { TestStatusTxt = "Canceled"; return false; }
                try
                {

                    Connect();
                    sucesfull_connections++;

                    Thread.Sleep(250);

                    connected = true;
                    TestStatusTxt = $"Connection successful #{sucesfull_connections} after {stopWatch.Elapsed.ToString(@"m\:ss")}";
                    if (sucesfull_connections >= 2)
                    {
                        break;
                    }
                    else
                    {
                        Dispose();
                    }
                }
                catch (Exception ex)
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
