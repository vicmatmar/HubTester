using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubTester.Tests
{
    public class Shutdown : TestBase
    {
        public override bool Run()
        {
            WriteCommand("poweroff");

            return true;
        }
    }
}
