//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExamsManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class eeduecert
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public eeduecert()
        {
            this.courses = new HashSet<cours>();
        }
    
        public int id { get; set; }
        public string Master { get; set; }
        public string Ikey { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cours> courses { get; set; }
    }
}
