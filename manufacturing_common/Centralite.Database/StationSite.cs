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
    
    public partial class StationSite
    {
        public string StationMac { get; set; }
        public int ProductionSiteId { get; set; }
    
        public virtual ProductionSite ProductionSite { get; set; }
    }
}
