using Centralite.Common.Enumerations;
using Centralite.Common.Interfaces;
using Centralite.Common.Printers;

namespace Centralite.Common.Factories
{
    public static class PrinterFactory
    {
        public static IPrinter CreatePrinter(PrinterType printerType)
        {
            IPrinter printer = null;

            switch(printerType)
            {
                case PrinterType.BradyPrinter:
                    printer = new Printers.BradyPrinter();
                    break;
                case PrinterType.ZebraPrinter:
                    printer = new ZebraPrinter();
                    break;
            }

            return printer;
        }
    }
}
