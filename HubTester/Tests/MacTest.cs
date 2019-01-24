using System.Globalization;
using System.Runtime.Serialization;

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
                    // Call MAC address script on board passing in generated MacAddress
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
