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
    
    public partial class LoadTest
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string Command { get; set; }
        public string ErrorMessage { get; set; }
    
        public virtual Test Test { get; set; }
    }
}
