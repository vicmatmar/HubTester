using System.Globalization;
using System.Text.RegularExpressions;
using Centralite.Database;
using System.Linq;
using MacUtility;

namespace HubTester.Tests
{
    /// <summary>
    /// /config/mac1 => 7C:66:9D:38:6A:FF
    /// </summary>
    public class MacTest : TestBase
    {
        private const string INVALID_MAC_ADDRESS = "000000000000";
        private long StartAddress;
        private long EndAddress;

//        public string MacAddress { get; set; }

        public MacTest() : base()
        {
            StartAddress = -1;
            EndAddress = -1;
        }

        public MacTest(string startBlock, string endBlock) : base()
        {
            startBlock = startBlock.Replace(":", "").Replace("-", "").Replace("=", "").Trim();
            endBlock = endBlock.Replace(":", "").Replace("-", "").Replace("=", "").Trim();

            if (!long.TryParse(startBlock, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out StartAddress))
            {
                // Invalid Start Address
                StartAddress = -1;
            }

            if (!long.TryParse(endBlock, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out EndAddress))
            {
                // Invalid End Address
                EndAddress = -1;
            }
        }

        /// <summary>
        /// Checks the hub mac is set
        /// </summary>
        /// <param name="reboot">will force hub to re-start if mac does not match</param>
        /// <returns>true if mac matches</returns>
        bool verifyIFConfig(bool reboot=false)
        {
            // Let's verify 
            Regex regex = new Regex(@"(eth0\s+.*encap:Ethernet\s+HWaddr\s+)(([0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2})");
            string line = WriteCommand("ifconfig");
            Match m = regex.Match(line);
            if(!m.Success || m.Groups.Count < 2)
            {
                TestErrorTxt = $"Unable to parse MAC from ifconfig command output:\r\n{line}";
                return false;
            }

            string ifmac = m.Groups[2].Value.ToUpper();
            string expmac = TestSequence.HUB_MAC_ADDR.ToUpper();
            if(ifmac != expmac && reboot)
            {
                WriteCommand("shutdown -r -t secs now");

                bool rebooted = false;
                for(int i = 0; i < 10; i++)
                {
                    try
                    {
                        Connect();
                        rebooted = true;
                        break;
                    }
                    catch { };
                }

                if(!rebooted)
                {
                    TestErrorTxt = $"Unable to restart hub";
                    return false;
                }
            }

            line = WriteCommand("ifconfig");
            m = regex.Match(line);
            if (!m.Success || m.Groups.Count < 2)
            {
                TestErrorTxt = $"Unable to parse MAC from ifconfig command output:\r\n{line}";
                return false;
            }
            ifmac = m.Groups[2].Value.ToUpper();

            TestStatusTxt = $"ifconfig MAC: {ifmac}";


            return (ifmac == expmac);

        }

        public override bool Run()
        {
            // Check if a file already exists and extract info if it does
            string onBoardMac = INVALID_MAC_ADDRESS;
            string hub_file_mac = WriteCommand($"cat /config/mac1");

            string mmac = MacAddressGenerator.ExtractMacString(hub_file_mac);
            if (mmac != null)
                onBoardMac = mmac;

            // Check it is in the database
            MacAddress dbmac = null;
            if (onBoardMac != INVALID_MAC_ADDRESS)
            {
                try
                {
                    dbmac = DataUtils.GetMacAddress(onBoardMac);
                }
                catch { };
                if (dbmac != null)
                {
                    TestStatusTxt = $"MAC address already assigned {onBoardMac} on {dbmac.Date.ToShortDateString()}";

                    dbmac = DataUtils.GetMacAddress(onBoardMac);
                    TestSequence.HUB_MAC_ADDR = MacAddressGenerator.LongToStr(dbmac.MAC);
                    return verifyIFConfig();
                }
            }

            // If we got this far, we need to check whether hub already in database with a mac
            // For that we need the hub eui.  We use the testSequence eui property
            //HUB_EUI = TestSequence.HUB_EUI;
            if (string.IsNullOrEmpty(TestSequence.HUB_EUI))
            {
                TestErrorTxt = "HUB_EUI needs to be set before this test";
                return false;
            }
            JiliaHub dbhub = null;
            try
            {
                dbhub = DataUtils.GetHub(TestSequence.HUB_EUI);
            }
            catch { };
            if(dbhub != null)
            {
                // This hub was previously tested, use this mac...
                // So why its local mac file not set???
                TestStatusTxt = $"MAC address already assigned {dbhub.Mac} during test on {dbhub.Timestamp.ToShortDateString()}";

                // Need to create the hub file
                WriteCommand($"echo $'{dbhub.Mac}' > /config/mac1");

                dbmac = DataUtils.GetMacAddress(dbhub.Mac);
                TestSequence.HUB_MAC_ADDR = MacAddressGenerator.LongToStr(dbmac.MAC);
                return verifyIFConfig();
            }

            // OK, so new hub, generate one mac address for it
            TestStatusTxt = "Generating MAC address";
            if (StartAddress == -1 || EndAddress == -1)
            {
                TestErrorTxt = $"Invalid start ({StartAddress}) or end ({EndAddress}) address";
                return false;
            }
            else
            {
                string macstr = MacAddressGenerator.Generate(StartAddress, EndAddress);
                if (macstr != INVALID_MAC_ADDRESS)
                {
                    WriteCommand($"echo $'{macstr}' > /config/mac1");

                    dbmac = DataUtils.GetMacAddress(macstr);
                    TestSequence.HUB_MAC_ADDR = MacAddressGenerator.LongToStr(dbmac.MAC);
                    return verifyIFConfig(true);
                }
                else
                {
                    TestErrorTxt = $"Unable to generate mac address";
                    return false;
                }
            }

        }
    }
}
