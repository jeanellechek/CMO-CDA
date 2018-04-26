using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMO101.Models
{
    public class _911DRO
    {
        public int caseID { get; set; }
        public string informantName { get; set; }
        public int informantNumber { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime timeStamp { get; set; }
        public int crisisLevel { get; set; }

    }
}