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

            return base.Setup();
        }
        public override bool Run()
        {
            using (var ctx = new ManufacturingStoreEntities())
            {
                JiliaHub dbjhub = new JiliaHub();

                dbjhub.EuiId = _dbeui.Id;
                dbjhub.MacAddressId = _dbmac.Id;

                dbjhub.Mac = MacAddressGenerator.LongToStr(_dbmac.MAC);
                TestStatusTxt = $"Hub MAC {dbjhub.Mac}";
                dbjhub.Bid = $"J{_dbeui.Id}";
                TestStatusTxt = $"Hub BId {dbjhub.Bid}";

                // Activation
                // /config/activation_key
                string line = WriteCommand("cat /config/activation_key");
                Regex regex = new Regex(@"([0-9,a-z,A-Z]{8})");
                Match match = regex.Match(line);
                if (!match.Success || match.Groups.Count < 2)
                {
                    TestErrorTxt = $"Unable to parse hub activation.  Line was {line}";
                    return false;
                }
                dbjhub.Activation = match.Groups[1].Value;
                TestStatusTxt = $"Hub activation {dbjhub.Activation}";

                // Uid
                line = WriteCommand("cat /data/run/.system");
                regex = new Regex(@"uuid: ([0-9,a-f]{8}-([0-9,a-f]{4}-){3}[0-9,a-f]{12})");
                match = regex.Match(line);
                if(!match.Success || match.Groups.Count < 2)
                {
                    TestErrorTxt = $"Unable to parse hub uuid.  Line was {line}";
                    return false;
                }
                dbjhub.Uid = match.Groups[1].Value;
                TestStatusTxt = $"Hub uuid {dbjhub.Uid}";

                // Insert
                ctx.JiliaHubs.Add(dbjhub);
                ctx.SaveChanges();
                TestStatusTxt = $"Hub dbrid {dbjhub.Id}";
            }
            return true;
        }
    }
}
