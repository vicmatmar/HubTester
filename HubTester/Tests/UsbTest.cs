
namespace HubTester.Tests
{
    /// <summary>
    /// To create a disk (I used ubuntu server)
    /// fdisk /dev/sdb, then delete all partitions (d)
    /// (o) create dos dsklabel
    /// (n) create partition primary
    /// (t) change type to (fat16 less than 32M option 4)
    /// (w) to write to disk
    /// To format: mkfs.vfat /dev/sdb1
    /// 
    /// mount /dev/sdb1 /mnt
    /// cd /mnt
    /// mkdir jssh
    /// cd jssh
    /// cat > sh2017, 1 Ctrl-D
    /// cd ~
    /// umount /mnt
    /// </summary>
    public class UsbTest : TestBase
    {
        const string TESTING_STRING = "Testing String";
        readonly string[] usb_drives = new string[] { "sda1", "sdb1"};

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
