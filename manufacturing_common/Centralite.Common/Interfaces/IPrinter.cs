using System.Collections.Generic;
using Centralite.Common.Models;

namespace Centralite.Common.Interfaces
{
    /// <summary>
    /// Interface that defines a printer that can be configured and printed to
    /// </summary>
    public interface IPrinter
    {     
        /// <summary>
        /// Configuration value for the printer. Should be in the specific format for the given printer.
        /// </summary>
        string Configuration { get; set; }

        /// <summary>
        /// Property which determines if the current printer has been configured or not
        /// </summary>
        bool Configured { get; }

        /// <summary>
        /// Prints the given labels
        /// </summary>
        /// <param name="labels">Collection of labels to be printed</param>
        void Print(IEnumerable<Label> labels);
    }
}
