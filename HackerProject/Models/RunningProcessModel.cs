using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.Models
{
    public class RunningProcessModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Details { get; set; }
        public double Version { get; set; }
        public long Cpu { get; set; }
        public long Ram { get; set; }
        public double Bw { get; set; }
        public string Ip { get; set; }
        public string Complete { get; set; }
        public string Connect { get; set; }
        public string Bounce { get; set; }
        public string Time { get; set; }
        public string Kill { get; set; }

    }
}
