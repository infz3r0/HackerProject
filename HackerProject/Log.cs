using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject
{
    public class Log
    {
        private string time;
        private string logs;
        private byte alert;

        public Log()
        {

        }

        public string Time { get => time; set => time = value; }
        public string Logs { get => logs; set => logs = value; }
        public byte Alert { get => alert; set => alert = value; }
    }
}
