using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace HubTests.Tests
{
    public class UsbTest : TestBase
    {
        private const string TESTING_STRING = "Testing String";

        private string userPrompt;

        public UsbTest(string userPrompt = null) : base()
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
                    WriteLine("umount /mnt");
                    Thread.Sleep(50);

                    string line = ReadToEnd();

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
                WriteLine("mount /dev/sda1 /mnt");
                Thread.Sleep(50);
                line = ReadLine();
                line = ReadLine();

                TestStatusTxt = line;

                WriteLine("echo {0} > /mnt/test.txt", TESTING_STRING);
                Thread.Sleep(50);
                line = ReadLine();
                line = ReadLine();

                TestStatusTxt = line;

                WriteLine("cat /mnt/test.txt");
                Thread.Sleep(50);

                while (!line.Contains("cat /mnt/test.txt"))
                {
                    line = ReadLine();
                }

                line = ReadLine();
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
                WriteLine("umount /mnt");
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
