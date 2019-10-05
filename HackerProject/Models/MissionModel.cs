using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.Models
{
    public class MissionModel
    {
        public string TargetIP { get; set; }
        public string Type { get; set; }
        public string FileID { get; set; }
        public string Reward { get; set; }
        public string Details { get; set; }
        public string Complete { get; set; }
        public string Mark { get; set; }
    }
}
