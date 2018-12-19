using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Centralite.Common.Enumerations;
using Centralite.Common.Interfaces;
using Centralite.Common.Models;
using Centralite.Common.Factories;

namespace Centralite.Services
{
    [Export(typeof(IHostPrintingService))]
    [Export(typeof(IPluginPrinterService))]
    public class PrintingService : IHostPrintingService, IPluginPrinterService
    {
        private List<Label> Labels = new List<Label>();
        private IPrinter printer;

        public bool Configured
        {
            get
            {
                return printer.Configured;
            }
        }

        public void AddLabel(int serialNum, byte[] eui, int tester, DateTime date, int station, int site, ushort firmwareVersion, Database.Product product, int hwRevision, int bomRevision)
        {
            Labels.Add(new Label(serialNum, eui, tester, date, station, site, firmwareVersion, product, hwRevision, bomRevision));
        }

        public string Configuration
        {
            get
            {
                return printer?.Configuration;
            }

            set
            {
                printer.Configuration = value;
            }
        }

        public void Print()
        {
            printer?.Print(Labels);
            Labels.Clear();
        }

        public void RegisterPrinterType(PrinterType printerType)
        {
            // Incase there are unprinted labels when changing printers.
            Labels.Clear();
            this.printer = PrinterFactory.CreatePrinter(printerType);
        }
    }
}
