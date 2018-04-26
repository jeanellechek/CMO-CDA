using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMO101.Models
{
    public class efDRO
    {
        public int incident_id { get; set; }
        public DateTime report_timestamp { get; set; }
        public int casualty { get; set; }
        public string damage_property { get; set; }
        public string actions_taken { get; set; }
    }
}