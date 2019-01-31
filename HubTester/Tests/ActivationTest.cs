using Centralite.Database;
using System.Text.RegularExpressions;
using System.Threading;

using MacUtility;

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
            if (string.IsNullOrEmpty(TestSequence.HUB_EUI))
            {
                TestErrorTxt = $"Hub EUI needs to be set before this test";
                return false;
            }
            Regex regex = new Regex(@"[0-9a-fA-F]{16}");
            if (regex.Matches(TestSequence.HUB_EUI).Count != 1)
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
            _dbmac = DataUtils.GetMacAddress(TestSequence.HUB_MAC_ADDR);

            //return base.Setup();
            return true;
        }
        public override bool Run()
        {
            using (var ctx = new ManufacturingStoreEntities())
            {
                JiliaHub dbjhub = new JiliaHub();

                dbjhub.EuiId = _dbeui.Id;
                dbjhub.MacId = _dbmac.Id;

                dbjhub.Mac = MacAddressGenerator.LongToStr(_dbmac.MAC);
                dbjhub.Bid = $"J{_dbmac.Id}";

                dbjhub.Activation = "test";
                dbjhub.Uid = "test6577-67dd-40c5-971a-ff3113520000";

                

                ctx.JiliaHubs.Add(dbjhub);
                ctx.SaveChanges();
            }
            return true;
        }
    }
}
