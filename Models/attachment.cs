using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamsManagement.Models
{
    public class attachment
    {
        public int eexmaster_id { get; set; }
        public int student_seat_number { get; set; }
        public string description { get; set; }
        public string fileData { get; set; }
    }
}