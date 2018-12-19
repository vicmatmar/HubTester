namespace Centralite.Common.Enumerations
{
    /// <summary>
    /// Identifies the status that a device is in when attempting to print
    /// </summary>
    public enum DevicePrintStatus
    {
        /// <summary>
        /// Default value of a device before printing.
        /// Indicates there have been no attempts to print the device yet
        /// </summary>
        Default,

        /// <summary>
        /// Value of a device when there already exists a serial number for a device.
        /// Need to allow the user to decide what actions should be taken to resolve the issue.
        /// Indicates there was a serial number conflict for the device.
        /// </summary>
        Conflict,

        /// <summary>
        /// Value of a device when the user indicates they wish to create a new serial number for the device.
        /// Indicates the device has attempted to be printed, and if a conflict occured, then the user wishes to overwrite the old serial number
        /// </summary>
        Generate,

        /// <summary>
        /// Value of a device when the user indicates they do not wish to overwrite the device serial number, but instead wish to update the Tester value and Date
        /// Indicates the device has attempted to be printed and a conflict occured
        /// </summary>
        Update,

        /// <summary>
        /// Value of a device when the user indicates they do not wish to overwrite the device serial number, tester, or date
        /// Indicates the device has attempted to be printed and a conflict occured
        /// </summary>
        Reprint,

        /// <summary>
        /// Value of a device when the user indicates they do not want to print or change the device.
        /// Indicates device has attempted to be printed and a conflict occured
        /// </summary>
        Nothing,

        /// <summary>
        /// Value of a device when it has been successfully printed
        /// Indicates the device is finished printing
        /// </summary>
        Printed,

        /// <summary>
        /// Value of a device when it has been unsuccessfully printed
        /// Indicates the device has failed printing
        /// </summary>
        Failed
    }
}
