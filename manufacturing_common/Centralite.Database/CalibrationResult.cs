//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Centralite.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class CalibrationResult
    {
        public int Id { get; set; }
        public int EuiId { get; set; }
        public Nullable<int> VoltageGain { get; set; }
        public Nullable<int> CurrentGain { get; set; }
        public System.DateTime DateCalibrated { get; set; }
        public int MachineId { get; set; }
    
        public virtual EuiList EuiList { get; set; }
        public virtual TestStationMachine TestStationMachine { get; set; }
    }
}
