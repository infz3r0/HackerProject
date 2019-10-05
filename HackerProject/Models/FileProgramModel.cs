using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.Models
{
    public class FileProgramModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public double Version { get; set; }
        public long Size { get; set; }
        public string Run { get; set; }
        public string Delete { get; set; }
        public string Upload { get; set; }
        public string Download { get; set; }
        public string Hide { get; set; }
        public string Unhide { get; set; }
        public string Public { get; set; }
        public string Private { get; set; }
        public string Encrypt { get; set; }
        public string Decrypt { get; set; }
    }
}
