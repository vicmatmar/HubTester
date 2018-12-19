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
    
    public partial class ProductionSite
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductionSite()
        {
            this.EuiLists = new HashSet<EuiList>();
            this.ProductionSiteProductJoins = new HashSet<ProductionSiteProductJoin>();
            this.StationSites = new HashSet<StationSite>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public bool LoadRangeTest { get; set; }
        public bool RunIct { get; set; }
        public bool RunRangeTest { get; set; }
        public bool LoadApplication { get; set; }
        public bool ForceChannel { get; set; }
        public bool Erase { get; set; }
        public bool EnableFirmwareChange { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EuiList> EuiLists { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductionSiteProductJoin> ProductionSiteProductJoins { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StationSite> StationSites { get; set; }
    }
}
