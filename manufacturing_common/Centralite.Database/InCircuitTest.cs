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
    
    public partial class InCircuitTest
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Nullable<bool> ExternalFlash { get; set; }
    
        public virtual Test Test { get; set; }
    }
}