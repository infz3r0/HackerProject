using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.Models
{
    public class StatusModel
    {
        public string CurCPU { get; set; }
        public string MaxCPU { get; set; }
        public string PerCPU { get; set; }
        public string CurRAM { get; set; }
        public string MaxRAM { get; set; }
        public string PerRAM { get; set; }
        public string CurHDD { get; set; }
        public string MaxHDD { get; set; }
        public string PerHDD { get; set; }
        public string CurBandwidth { get; set; }
        public string MaxBandwidth { get; set; }
        public string PerBandwidth { get; set; }
        
    }
}
