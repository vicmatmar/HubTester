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
    
    public partial class RangeTest
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int Channel { get; set; }
        public int PacketsSent { get; set; }
        public int PacketsFailed { get; set; }
        public double TxRssiAverage { get; set; }
        public double TxLqiAverage { get; set; }
        public double RxRssiAverage { get; set; }
        public double RxLqiAverage { get; set; }
        public int RequiredValidSamples { get; set; }
        public int MaxAttempts { get; set; }
        public double FailureThreshold { get; set; }
        public double TxLqiThreshold { get; set; }
        public double TxRssiThreshold { get; set; }
        public double RxLqiThreshold { get; set; }
        public double RxRssiThreshold { get; set; }
    
        public virtual Test Test { get; set; }
    }
}
