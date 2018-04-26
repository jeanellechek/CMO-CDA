using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMO101.Models
{
    public class efDTO
    {
        public int caseID { get; set; }
        public string crisisLV { get; set; }
        public string actions { get; set; }
        public string unitsDeployed { get; set; }

        public DateTime DT { get; set; }
        public string informantName { get; set; }
        public int informantPhone { get; set; }
        public string location { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}