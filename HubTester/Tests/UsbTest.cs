using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class UsbTest : TestBase
    {
        private const string TESTING_STRING = "Testing String";

        private string userPrompt;

        public UsbTest(string ipAddress, string sshKeyFile, string userPrompt = null) : base(ipAddress, sshKeyFile)
        {
            this.userPrompt = userPrompt;
        }

        public override bool Setup()
        {
            bool result = base.Setup();

            if (result)
            {
                try
                {
                    streamWriter.WriteLine("umount /mnt");
                    Thread.Sleep(50);

                    string line = streamReader.ReadToEnd();

                    if (!string.IsNullOrEmpty(userPrompt))
                    {
                        MessageBox.Show(userPrompt);
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public override bool Run()
        {
            bool result = true;
            string line = "";

            try
            {
                streamWriter.WriteLine("mount /dev/sda1 /mnt");
                Thread.Sleep(50);
                line = streamReader.ReadLine();
                line = streamReader.ReadLine();

                TestStatusTxt = line;

                streamWriter.WriteLine("echo {0} > /mnt/test.txt", TESTING_STRING);
                Thread.Sleep(50);
                line = streamReader.ReadLine();
                line = streamReader.ReadLine();

                TestStatusTxt = line;

                streamWriter.WriteLine("cat /mnt/test.txt");
                Thread.Sleep(50);

                while (!line.Contains("cat /mnt/test.txt"))
                {
                    line = streamReader.ReadLine();
                }

                line = streamReader.ReadLine();
                if (line == TESTING_STRING)
                {
                    TestStatusTxt = "Test Passed";
                }
                else
                {
                    TestStatusTxt = "Test Failed";
                    result = false;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public override bool TearDown()
        {
            bool tearDownResult = true;

            try
            {
                streamWriter.WriteLine("umount /mnt");
                Thread.Sleep(50);

                tearDownResult &= base.TearDown();
            }
            catch
            {
                tearDownResult = false;
            }

            return tearDownResult;
        }
    }
}
