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
    
    public partial class cours
    {
        public int id { get; set; }
        public string name { get; set; }
        public Nullable<double> min_grade { get; set; }
        public Nullable<double> max_grade { get; set; }
        public Nullable<int> sho3ba_id { get; set; }
        public Nullable<int> shada_id { get; set; }
        public Nullable<int> year_id { get; set; }
        public Nullable<bool> is_main { get; set; }
    
        public virtual edducyear edducyear { get; set; }
        public virtual eeduecert eeduecert { get; set; }
        public virtual eeduppx0 eeduppx0 { get; set; }
    }
}
