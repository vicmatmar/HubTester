using System;
using System.Collections.Generic;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using System.IO;

namespace Centralite.Common.Printers
{
    public class BradyPrinter : IPrinter
    {
        public bool Configured
        {
            get
            {
                return true;
            }
        }

        public string Configuration
        {
            get; set;
        }

        public bool Configure(string configurationString)
        {
            return true;
        }

        public void Print(IEnumerable<Label> labels)
        {
            foreach (var label in labels)
            {
                Centralite.BradyPrinter.BradyPrinter printer = new Centralite.BradyPrinter.BradyPrinter();

                string filename = "";
                string printString = "";

                filename = label.Product.ZplFile;

                using (StreamReader streamReader = File.OpenText(Properties.Settings.Default.LabelFilesDirectory + "//" + filename))
                {
                    printString = streamReader.ReadToEnd();
                }

                printString = printString.Replace("\r", "");

                string EUI = BitConverter.ToString(label.EUI).Replace("-", ":");
                string shortEUI = BitConverter.ToString(label.EUI).Replace("-", "");
                string serialNumber = string.Format("{0}{1:000000000}", label.Product.SerialNumberCode, label.SerialNumber);
                string infoLine = string.Format("{0:00} {1:MMddyy} {2:00} {3:00} {4:X4}  {5:00}", label.Tester, label.Date, label.Station, label.Site, label.FirmwareVersion, "B" + label.BomRevision);

                printString = string.Format(printString, serialNumber, EUI, label.Product.SKU, infoLine, label.EncodeLowesManufacturingInformation(), label.ProductionCountry, label.ProductionCountry.PadLeft(6), shortEUI);

                printer.LoadLabel(printString);

                printer.PrintLabel();
            }
        }
    }
}
