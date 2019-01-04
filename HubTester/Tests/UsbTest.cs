using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class UsbTest : TestBase
    {
        private const string TESTING_STRING = "Testing String";


        public UsbTest(string userPrompt = null) : base()
        {
        }

        public override bool Setup()
        {
            bool result = base.Setup();

            if (result)
            {
                WriteCommand("umount /mnt");
            }

            return result;
        }

        public override bool Run()
        {
            string line = "";

            line = WriteCommand("mount /dev/sda1 /mnt");
            line = WriteCommand($"echo {TESTING_STRING} > /mnt/test.txt");
            line = WriteCommand("cat /mnt/test.txt");

            if (line != TESTING_STRING)
                return false;

            return true;
        }

        public override bool TearDown()
        {
            bool result = true;

            WriteCommand("umount /mnt");

            result &= base.TearDown();

            return result;
        }
    }
}
