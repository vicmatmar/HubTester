using Centralite.Database;
using System.Text.RegularExpressions;
using System.Threading;

namespace HubTester.Tests
{
    public class ActivationTest : TestBase
    {
        EuiList _dbeui;
        MacAddress _dbmac;

        public ActivationTest() : base() { }

        public override bool Setup()
        {
            //  Make sure everything we need is set

            // EUI
            if(string.IsNullOrEmpty(TestSequence.HUB_EUI))
            {
                TestErrorTxt = $"Hub EUI needs to be set before this test";
                return false;
            }
            Regex regex = new Regex(@"[0-9a-fA-F]{16}");
            if(regex.Matches(TestSequence.HUB_EUI).Count != 1)
            {
                TestErrorTxt = $"Bad Hub EUI {TestSequence.HUB_EUI} provided";
                return false;
            }
            _dbeui = DataUtils.GetEUI(TestSequence.HUB_EUI);

            // MAC
            if (string.IsNullOrEmpty(TestSequence.HUB_MAC_ADDR))
            {
                TestErrorTxt = $"Hub MAC address needs to be set before this test";
                return false;
            }
            regex = new Regex(@"([0-9a-fA-F]{2}:){5}[0-9a-fA-F]{2}");
            if (regex.Matches(TestSequence.HUB_MAC_ADDR).Count != 1)
            {
                TestErrorTxt = $"Bad Hub MAc address {TestSequence.HUB_MAC_ADDR} provided";
                return false;
            }
            // not used, done to make sure is in db
            _dbmac = DataUtils.GetMacAddress(TestSequence.HUB_MAC_ADDR);  

            //return base.Setup();
            return true;
        }
        public override bool Run()
        {
            return true;
        }
    }
}
