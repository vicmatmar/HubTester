﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTester.Tests
{
    public class BatteryTest : TestBase
    {
        private const int RETRY_TIMEOUT = 5;

        public BatteryTest() : base() { }

        public override bool Run()
        {
            TestStatusTxt = "Running Battery Test";
            bool result = false;

            try
            {
                bool InitialConnectionResult = TryConnection();

                if (InitialConnectionResult)
                {
                    MessageBox.Show("Remove AC power, ensure battery power is on.");

                    result = TryConnection();
                }
            }
            catch
            {
                result = false;
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

        private bool TryConnection()
        {
            bool result = true;
            int retries = 0;
            string line = "";

            ReadToEnd();
            WriteLine("spud");
            Thread.Sleep(500);

            line = ReadToEnd();
            while (retries < RETRY_TIMEOUT)
            {
                if (line.Contains("Yes, this is spud."))
                {
                    break;
                }

                retries++;
                line = ReadToEnd();
                Thread.Sleep(500);
            }

            if (retries >= RETRY_TIMEOUT)
            {
                result = false;
            }

            return result;
        }
    }
}
