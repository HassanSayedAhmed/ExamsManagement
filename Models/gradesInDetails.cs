using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamsManagement.Models
{
    public class gradesInDetails
    {
        public int? seat_id { get; set; }
        public string student_name { get; set; }
        public string school_name { get; set; }
        public string section { get; set; }
        public string to_destination { get; set; }
        public double? price { get; set; }
        public string date { get; set; }
        public string unique_registration { get; set; }
        public string dor { get; set; }
        public string year { get; set; }
        public double? max_grade { get; set; }
        public double? min_grade { get; set; }
        public double? total_grade { get; set; }
        public string transfer_number { get; set; }
        public string eldoor { get; set; }
        
        public List<coursesWithGrades> coursesWithGradesMain { get; set; }
        public List<coursesWithGrades> coursesWithGradesOptional { get; set; }
    }
}