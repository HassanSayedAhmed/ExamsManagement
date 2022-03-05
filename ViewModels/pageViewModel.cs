using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExamsManagement.ViewModels
{
    public class pageViewModel
    {
        public int id { get; set; }
        public string eegov { get; set; }
        public string eedusection { get; set; }
        public string eschool { get; set; }
        public string elevel { get; set; }
        public string estatus { get; set; }
        public string eyear { get; set; }
        public string eeduppx0 { get; set; }
        public string edocref { get; set; }
        public int? epage { get; set; }
        public string erowref { get; set; }
        public int? estudentfrom { get; set; }
        public int? estudentto { get; set; }
        public string eedulictype { get; set; }
        public string eebook { get; set; }
        public string eeCount { get; set; }
        public bool DocThere { get; set; }
        public int UserID { get; set; }
    }
}