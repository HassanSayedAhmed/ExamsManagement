using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamsManagement.Models
{
    public class coursesWithGrades
    {
        public double? max_grade { get; set; }
        public double? min_grade { get; set; }
        public double? grade1 { get; set; }
        public string course_name { get; set; }
        public int? student_id { get; set; }
        public bool? is_main { get; set; }
    }
}