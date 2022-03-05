using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamsManagement.Models
{
    public class successStatement
    {
        public string date { get; set; }
        public string shahada { get; set; }
        public string eldoor { get; set; }
        public string sho3ba { get; set; }
        public string year { get; set; }
        public int? seat_id { get; set; }
        public string student_name { get; set; }
        public string school_name { get; set; }
        public string section { get; set; }
        public double? student_total_grades { get; set; }
        public double? max_grades { get; set; }
        public string to_destination { get; set; }
        public double? price { get; set; }
        public string transfer_number { get; set; }
        public string unique_registration { get; set; }
    }
}