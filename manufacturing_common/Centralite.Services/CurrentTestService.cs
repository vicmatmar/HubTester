using Centralite.Common.Enumerations;
using Centralite.Common.Interfaces;
using Centralite.Common.Utilities;
using Centralite.CurrentSensor;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Centralite.Services
{
    [Export(typeof(ICurrentTestService))]
    public class CurrentTestService : ICurrentTestService
    {
        public IEnumerable<double> GetCurrentResults(CurrentTestType currentTestType)
        {
            switch (currentTestType)
            {
                case CurrentTestType.CINA_CurrentTest:
                    using (var currentTester = new CINA219_CurrentTester())
                    {
                        return currentTester.GetCurrentResults();
                    }
                case CurrentTestType.NI_CurrentTest:
                default:
                    return NI_CurrentTester.GetCurrentResults();
            }
        }
    }
}
