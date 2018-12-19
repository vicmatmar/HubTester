using Centralite.Common.Enumerations;
using System.Collections.Generic;

namespace Centralite.Common.Interfaces
{
    public interface ICurrentTestService
    {
        IEnumerable<double> GetCurrentResults(CurrentTestType currentTestType);
    }
}
