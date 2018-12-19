using System.Collections.Generic;
using NationalInstruments.DAQmx;
using System;

namespace Centralite.Common.Utilities
{
    public static class NI_CurrentTester
    {
        private static string[] GetAnalogChannels(string[] devices)
        {
            string[] analogChannels = null;

            foreach (var device in devices)
            {
                analogChannels = DaqSystem.Local.LoadDevice(device).GetPhysicalChannels(PhysicalChannelTypes.AI, PhysicalChannelAccess.External);
            }

            return analogChannels;
        }

        private static void AddAnalogChannelsToTask(Task analogTask, string[] analogChannels)
        {
            foreach (var analogChannel in analogChannels)
            {
                analogTask.AIChannels.CreateVoltageChannel(analogChannel.ToString(),
                    analogChannel.ToString().Replace('/', '_'),
                    AITerminalConfiguration.Rse,
                    Convert.ToDouble(-10.00),
                    Convert.ToDouble(10.00),
                    AIVoltageUnits.Volts);
            }
        }

        private static double[] FormatResults(double[] currents)
        {
            var results = new List<double>();

            foreach (var current in currents)
            {
                results.Add((Math.Pow(10.0, (-1.0 * current))) * 1000.0 * 1000.0);
            }

            return results.ToArray();
        }

        public static IEnumerable<double> GetCurrentResults()
        {
            var analogTask = new Task();
            var devices = DaqSystem.Local.Devices;

            var analogChannels = GetAnalogChannels(devices);

            AddAnalogChannelsToTask(analogTask, analogChannels);
            analogTask.Control(TaskAction.Verify);

            var channelReader = new AnalogMultiChannelReader(analogTask.Stream);

            double[] results = channelReader.ReadSingleSample();

            return FormatResults(results);
        }
    }
}
