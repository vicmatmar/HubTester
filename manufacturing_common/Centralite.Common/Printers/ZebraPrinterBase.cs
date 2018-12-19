using System;
using System.Collections.Generic;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using System.Net.Sockets;
using System.Net;

namespace Centralite.Common.Printers
{
    public abstract class ZebraPrinterBase : IPrinter
    {
        private const int ZebraPrinterTcpPort = 9100;

        public bool Configured
        {
            get
            {
                return string.IsNullOrEmpty(Properties.Settings.Default.ZebraPrinterSetting);
            }
        }

        public string Configuration
        {
            get
            {
                return Properties.Settings.Default.ZebraPrinterSetting;
            }
            set
            {
                Properties.Settings.Default.ZebraPrinterSetting = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool Configure(string configurationString)
        {
            IPAddress temp;

            if (IPAddress.TryParse(configurationString, out temp))
            {
                Properties.Settings.Default.ZebraPrinterSetting = configurationString;
                Properties.Settings.Default.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        public abstract void Print(IEnumerable<Label> labels);

        protected virtual void SendZplToPrinter(string zpl)
        {
            TcpClient client = new TcpClient();
            client.Connect(Properties.Settings.Default.ZebraPrinterSetting, ZebraPrinterTcpPort);

            System.IO.StreamWriter writer = new System.IO.StreamWriter(client.GetStream());
            writer.Write(zpl);
            writer.Flush();

            writer.Close();
            client.Close();
        }
    }
}
