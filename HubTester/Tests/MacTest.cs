using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

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

        public string MacAddress { get; set; }

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

        public override bool Run()
        {
            // Check if a file already exists and extract info if it does
            string onBoardMac = INVALID_MAC_ADDRESS;
            string rs = WriteCommand($"cat /config/mac1");
            Regex regex = new Regex(@"[0-9,a-f,A-f]{12}");
            Match match = regex.Match(rs);
            if (match.Success)
            {
                onBoardMac = match.Value;
            }
            else
            {
                regex = new Regex(@"([0-9,a-f,A-f]{2}:){5}[0-9,a-f,A-f]{2}");
                match = regex.Match(rs);
                if (match.Success)
                {
                    onBoardMac = match.Value;
                }
            }

            if(onBoardMac != INVALID_MAC_ADDRESS)
            {
                TestStatusTxt = $"MAC address already assigned: {onBoardMac}";
                return true;
            }


            TestStatusTxt = "Generating MAC address";
            bool result = false;

            if (StartAddress == -1 || EndAddress == -1)
            {
                TestErrorTxt = "Invalid start or end address";
                result = false;
            }
            else
            {
                MacAddress = MacUtility.MacAddressGenerator.Generate(StartAddress, EndAddress);

                if (MacAddress != INVALID_MAC_ADDRESS)
                {
                    WriteCommand($"echo $'{MacAddress}' > /config/mac1");
                }
                else
                {
                    result = false;
                }
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
    }
}
