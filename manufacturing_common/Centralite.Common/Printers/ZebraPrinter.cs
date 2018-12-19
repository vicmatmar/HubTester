using System;
using System.Collections.Generic;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using System.IO;

namespace Centralite.Common.Printers
{
    public class ZebraPrinter : ZebraPrinterBase
    {
        public override void Print(IEnumerable<Label> labels)
        {
            foreach(var label in labels)
            {
                string filename = "";
                string zplString = "";

                filename = label.Product.ZplFile;

                using (StreamReader streamReader = File.OpenText(Properties.Settings.Default.LabelFilesDirectory + "//" + filename))
                {
                    zplString = streamReader.ReadToEnd();
                }

                string EUI = BitConverter.ToString(label.EUI).Replace("-", ":");
                string shortEUI = BitConverter.ToString(label.EUI).Replace("-", "");
                string serialNumber = string.Format("{0}{1:000000000}", label.Product.SerialNumberCode, label.SerialNumber);
                string infoLine = string.Format("{0:00} {1:MMddyy} {2:00} {3:00} {4:X4}  {5:00}", label.Tester, label.Date, label.Station, label.Site, label.FirmwareVersion, "B" + label.BomRevision);
                string serialZplCode = string.Format(zplString, serialNumber, EUI, label.Product.SKU, infoLine, label.EncodeLowesManufacturingInformation(), label.ProductionCountry, label.ProductionCountry.PadLeft(6), shortEUI);

                this.SendZplToPrinter(serialZplCode);
            }
        }
    }
}
