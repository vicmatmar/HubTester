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
    
    public partial class JiliaHub
    {
        public int Id { get; set; }
        public string Bid { get; set; }
        public string Mac { get; set; }
        public string Uid { get; set; }
        public string Activation { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int EuiId { get; set; }
        public int MacId { get; set; }
    
        public virtual EuiList EuiList { get; set; }
        public virtual MacAddress MacAddress { get; set; }
    }
}
