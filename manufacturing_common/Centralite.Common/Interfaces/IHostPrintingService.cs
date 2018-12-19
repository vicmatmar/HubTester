using System;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface which defines a way for a plugin to register the type of printer it uses, and the host application to be able to add labels and print to that printer
    /// </summary>
    public interface IHostPrintingService
    {
        /// <summary>
        /// Used by host application to add a new label to be printed by the printer
        /// </summary>
        /// <param name="serialNum"></param>
        /// <param name="eui"></param>
        /// <param name="tester"></param>
        /// <param name="date"></param>
        /// <param name="station"></param>
        /// <param name="site"></param>
        /// <param name="firmwareVersion"></param>
        /// <param name="product"></param>
        /// <param name="hwRevision"></param>
        /// <param name="bomRevision"></param>
        void AddLabel(int serialNum, byte[] eui, int tester, DateTime date, int station, int site, ushort firmwareVersion, Centralite.Database.Product product, int hwRevision, int bomRevision);

        /// <summary>
        /// Uses the registered printer to print all labels that have been added
        /// </summary>
        void Print();
    }
}
