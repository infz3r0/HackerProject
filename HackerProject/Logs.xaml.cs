using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Data;
using HtmlAgilityPack;
using System.Net;
using System.Threading;
using System.Windows.Threading;

namespace HackerProject
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : Window
    {
        public DataTable data = new DataTable();
        public List<Log> logs = new List<Log>();
        public static Thread InstanceCaller;
        public bool autoRefreshOn = false;

        public Logs()
        {
            InitializeComponent();

            LoadData();


            InstanceCaller = new Thread(new ParameterizedThreadStart(AutoRefresh));
        }

        public async void AutoRefresh(object oms)
        {
            int ms = (int)oms;
            while (true)
            {
                DataTable dt = await GetData();
                this.Dispatcher.Invoke(() => this.dgvLogs.DataContext = dt, DispatcherPriority.Normal);


                Thread.Sleep(ms);
            }
        }        

        private async Task GetLogs()
        {
            logs.Clear();
            int o = 0;
            int i = -1;

            while (true)
            {
                if (i >= 50)
                {
                    break;
                }

                string reqUri = MainWindow.domain + "index.php?action=gate&a2=logs&_o=" + o;
                string responseString = await MainWindow.GET(reqUri, MainWindow.cookies);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//td[@class='dbg' and @valign='top']");

                if (nodes == null)
                {
                    break;
                }                

                foreach (HtmlNode n in nodes)
                {
                    Log l = new Log();
                    l.Time = n.InnerText;
                    logs.Add(l);
                }

                HtmlNodeCollection nodes2 = doc.DocumentNode.SelectNodes(@"//textarea[@name='log_str']");

                
                foreach (HtmlNode n2 in nodes2)
                {
                    i++;
                    string s = n2.InnerText;
                    logs[i].Logs = s;
                    if (s.Contains("[localhost]") || s.Contains("Origin proxy") || s.Contains("[Kernel]"))
                    {
                        logs[i].Alert = 0;
                    }
                    else if (s.Contains("Download"))
                    {
                        logs[i].Alert = 1;
                    }
                    else
                    {
                        logs[i].Alert = 2;
                    }
                }

                o += 10;

            } //end while


        }

        public async void LoadData()
        {
            data = await GetData();
            dgvLogs.DataContext = data.DefaultView;
        }

        private async Task<DataTable> GetData()
        {
            await GetLogs();
            DataTable dt = new DataTable();
            dt.Columns.Add("Time");
            dt.Columns.Add("Log");
            dt.Columns.Add("Alert");
            foreach (Log l in logs)
            {
                DataRow row = dt.NewRow();
                row["Time"] = l.Time;
                row["Log"] = l.Logs;
                row["Alert"] = l.Alert;

                dt.Rows.Add(row);
            }

            return dt;
        }

        private void ChangeColor()
        {
            foreach (DataRow r in dgvLogs.Items)
            {

            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void btnAutoRefresh_Click(object sender, RoutedEventArgs e)
        {

        }


        //
    }
}
