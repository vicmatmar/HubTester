using System;
using System.Threading;

namespace HubTester.Tests
{
    class LedBlinker : TestBase
    {
        string _color;
        public LedBlinker(string color) : base()
        {
            Color = color;
        }

        public string Color { get => _color; set => _color = value; }

        public override bool Run()
        {
            bool result = base.Setup();
            if (!result)
                return false;

            try
            {
                WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "red"));
                WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "yellow"));
                WriteCommand("echo none > " + string.Format(LED_TRIGGER_PATH, "green"));

                WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "red"));
                WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "yellow"));
                WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, "green"));

                while (true)
                {
                    if (CancelToken.IsCancellationRequested)
                        break;

                    try
                    {
                        WriteCommand("echo 1 > " + string.Format(LED_BRIGHTNESS_PATH, Color), cmd_delay_ms: 100);

                        Thread.Sleep(250);

                        WriteCommand("echo 0 > " + string.Format(LED_BRIGHTNESS_PATH, Color), cmd_delay_ms: 100);

                        Thread.Sleep(250);
                    }
                    catch
                    {
                        result = false;
                        break; ;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                result = false;
            };

            base.TearDown();

            return result;
        }
    }
}
