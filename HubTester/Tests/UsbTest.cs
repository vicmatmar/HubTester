using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class UsbTest : TestBase
    {
        const string TESTING_STRING = "Testing String";
        string[] usb_drives = new string[] { "sda1", "sdb1"};

        public UsbTest() : base()
        {
        }

        public override bool Setup()
        {
            bool result = base.Setup();

            if (result)
                WriteCommand("umount /mnt");

            return result;
        }

        public override bool Run()
        {
            string line = "";

            foreach (string usb_drive in usb_drives)
            {

                line = WriteCommand($"mount /dev/{usb_drive} /mnt");
                if (line != "")
                {
                    TestErrorTxt = $"Unable to mount {usb_drive}.  Return: {line}";
                    return false;
                }

                line = WriteCommand($"echo {TESTING_STRING} > /mnt/test.txt");
                line = WriteCommand("cat /mnt/test.txt");
                if (line != TESTING_STRING)
                {
                    TestErrorTxt = $"Unable to write to {usb_drive}.  Return: {line}";
                    return false;
                }
            }

            return true;
        }

        public override bool TearDown()
        {
            bool result = true;

            string line = WriteCommand("umount /mnt");

            result &= base.TearDown();

            return result;
        }
    }
}
