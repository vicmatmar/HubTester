using Centralite.Common.Enumerations;
using System.Collections.Generic;
using System.Linq;

namespace Centralite.Common.Models
{
    public class ConflictMode
    {
        public string Name { get; set; }
        public string DisplayText { get; set; }
        public DevicePrintStatus DevicePrintStatus { get; set; }
        public bool SupervisorOnly { get; set; }

        private static List<ConflictMode> conflictModeList = new List<ConflictMode>();

        public static IEnumerable<ConflictMode> ConflictModes
        {
            get
            {
                if (!conflictModeList.Any())
                {
                    conflictModeList.Add(new ConflictMode()
                    {
                        Name = "Nothing",
                        DisplayText = "Do nothing, do not print or update device information",
                        DevicePrintStatus = DevicePrintStatus.Nothing,
                        SupervisorOnly = false
                    });

                    conflictModeList.Add(new ConflictMode()
                    {
                        Name = "Reprint",
                        DisplayText = "Change no information and reprint",
                        DevicePrintStatus = DevicePrintStatus.Reprint,
                        SupervisorOnly = false
                    });

                    conflictModeList.Add(new ConflictMode()
                    {
                        Name = "UpdateAndReprint",
                        DisplayText = "Update tester and date  only, and reprint",
                        DevicePrintStatus = DevicePrintStatus.Update,
                        SupervisorOnly = false
                    });

                    conflictModeList.Add(new ConflictMode()
                    {
                        Name = "Override",
                        DisplayText = "Assign new serial number",
                        DevicePrintStatus = DevicePrintStatus.Generate,
                        SupervisorOnly = true
                    });
                }
                return conflictModeList;
            }
        }
    }
}
