using Centralite.Common.Enumerations;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPluginPrinterService
    {
        /// <summary>
        /// Uses a printer of the specified type.
        /// Used by plugin.
        /// </summary>
        /// <param name="printerType"></param>
        void RegisterPrinterType(PrinterType printerType);

        /// <summary>
        /// Configuration of the printer. Should be in a particular format for the implementation of the printer
        /// </summary>
        string Configuration { get; set; }

        /// <summary>
        /// Property which determines if the current printer has been configured or not
        /// </summary>
        bool Configured { get; }
    }
}
