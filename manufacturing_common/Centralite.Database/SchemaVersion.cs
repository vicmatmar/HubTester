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
    
    public partial class SchemaVersion
    {
        public int Id { get; set; }
        public int MajorReleaseNumber { get; set; }
        public int MinorReleaseNumber { get; set; }
        public int PointReleaseNumber { get; set; }
        public string ScriptName { get; set; }
        public System.DateTime DateApplied { get; set; }
    }
}
