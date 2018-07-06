using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject
{
    public class Process
    {
        private string id;
        private string type;
        private string details;
        private double version;
        private long cpu;
        private long ram;
        private double bw;
        private string time;

        private string ip;

        private string complete;
        private string connect;
        private string bounce;


        public Process()
        {

        }

        public string Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        public string Details { get => details; set => details = value; }
        public double Version { get => version; set => version = value; }
        public long Cpu { get => cpu; set => cpu = value; }
        public long Ram { get => ram; set => ram = value; }
        public double Bw { get => bw; set => bw = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Complete { get => complete; set => complete = value; }
        public string Connect { get => connect; set => connect = value; }
        public string Bounce { get => bounce; set => bounce = value; }
        public string Time { get => time; set => time = value; }

        public static async Task Kill(string Id)
        {
            string reqUri = MainWindow.domain + "index.php?action=gate&a2=run&k_pid=" + Id;
            string responseString = await MainWindow.GET(reqUri, MainWindow.cookies);
        }
    }
}
