using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMO101.Models
{
    public class pmoDTO
    {
        public int caseID { get; set; }
        public string crisisLV { get; set; }
        public string actionToDo { get; set; }
        public DateTime DT { get; set; }
        public int casualties { get; set; }
        public string damagedProperties { get; set; }
        public string unitsDeployed { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }

        public string caseStatus { get; set; } 
    }
}