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
    /// Interaction logic for RunningSoftware.xaml
    /// </summary>
    public partial class RunningSoftware : Window
    {
        public DataTable data = new DataTable();
        public List<Process> runningSoft = new List<Process>();
        public static Thread InstanceCaller;
        public bool autoRefreshOn = false;

        public RunningSoftware()
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
                this.Dispatcher.Invoke(() => this.dgvRunningSoft.DataContext = dt, DispatcherPriority.Normal);


                Thread.Sleep(ms);
            }
        }

        private void AddSoftware(string decoded)
        {
            try
            {
                Process p = new Process();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(decoded);

                HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='snl']");
                p.Id = node.InnerText;

                node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='p1']");
                p.Type = node.InnerText;

                node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='sm2']");
                p.Version = Convert.ToDouble(node.InnerText);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//tr/td[@class='sm']");
                p.Details = nodes[0].InnerText;
                p.Cpu = Convert.ToInt64(nodes[1].InnerText);
                p.Ram = Convert.ToInt64(nodes[2].InnerText);
                p.Bw = Convert.ToDouble(nodes[3].InnerText);

                //Time
                string[] spliter = { "," };
                string[] splits = p.Details.Split(spliter, StringSplitOptions.None);
                if (splits.Length > 1)
                {
                    p.Time = splits[1];
                }

                //Analysis details
                doc.LoadHtml(nodes[0].InnerHtml);

                node = doc.DocumentNode.SelectSingleNode(@"//span[@class='green']");
                if (node != null)
                {
                    p.Ip = node.InnerText;
                }

                nodes = doc.DocumentNode.SelectNodes(@"//a");
                if (nodes != null)
                {
                    if (nodes.Count >= 1)
                    {
                        p.Complete = nodes[0].Attributes[0].Value;
                        if (nodes.Count >= 3)
                        {
                            p.Connect = nodes[1].Attributes[0].Value;
                            p.Bounce = nodes[2].Attributes[0].Value;
                        }
                    }

                }



                runningSoft.Add(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task RunningSoft()
        {
            runningSoft.Clear();
            int o = 0;

            while (true)
            {
                string reqUri = MainWindow.domain + "index.php?action=gate&a2=run&_o=" + o;
                string responseString = await MainWindow.GET(reqUri, MainWindow.cookies);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);

                HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//form[@name='frm_files']/table");

                HtmlNodeCollection childs = node.ChildNodes;
                string[] stringSeparators = new string[] { "'" };

                int count = 0;

                foreach (HtmlNode n in childs)
                {
                    if (n.Name.Equals("script") && n.InnerText.Contains("document.write"))
                    {
                        string text = n.InnerText;
                        string[] splits = text.Split(stringSeparators, StringSplitOptions.None);

                        string encoded = splits[1];
                        string decoded = WebUtility.UrlDecode(encoded);

                        AddSoftware(decoded);

                        count++;
                    }
                }

                if (count == 0)
                {
                    break;
                }

                o += 50;

            } //end while
            
        }

        public async void LoadData()
        {
            pgbLoad.Value = 0;
            data = await GetData();
            dgvRunningSoft.DataContext = data.DefaultView;
            pgbLoad.Value = 100;
        }

        private async Task<DataTable> GetData()
        {
            await RunningSoft();

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Type");
            dt.Columns.Add("Details");
            dt.Columns.Add("Version");
            dt.Columns.Add("Cpu");
            dt.Columns.Add("Ram");
            dt.Columns.Add("Bw");
            dt.Columns.Add("Time");
            dt.Columns.Add("Ip");
            dt.Columns.Add("Complete");
            dt.Columns.Add("Connect");
            dt.Columns.Add("Bounce");
            foreach (Process s in runningSoft)
            {
                DataRow row = dt.NewRow();
                row["Id"] = s.Id;
                row["Type"] = s.Type;
                row["Details"] = s.Details;
                row["Version"] = s.Version;
                row["Cpu"] = s.Cpu;
                row["Ram"] = s.Ram;
                row["Bw"] = s.Bw;
                row["Time"] = s.Time;
                row["Ip"] = s.Ip;
                row["Complete"] = s.Complete;
                row["Connect"] = s.Connect;
                row["Bounce"] = s.Bounce;

                dt.Rows.Add(row);
            }

            

            return dt;
        }

        private async Task Kill()
        {
            if (dgvRunningSoft.SelectedCells.Count <= 0)
            {
                return;
            }
            foreach (DataGridCellInfo cell in dgvRunningSoft.SelectedCells)
            {
                DataRow row = ((DataRowView)cell.Item).Row;
                string id = (string)row.ItemArray[0];

                await Process.Kill(id);
            }

            LoadData();
        }

        private async Task Complete()
        {
            if (dgvRunningSoft.SelectedCells.Count <= 0)
            {
                return;
            }
            foreach (DataGridCellInfo cell in dgvRunningSoft.SelectedCells)
            {
                DataRow row = ((DataRowView)cell.Item).Row;
                if (row.ItemArray[9] == DBNull.Value)
                {
                    continue;
                }
                string complete = (string)row.ItemArray[9];
                if (string.IsNullOrEmpty(complete))
                {
                    continue;
                }
                string uri = MainWindow.domain + complete;

                await MainWindow.GET(uri, MainWindow.cookies);
            }

            LoadData();
        }

        private async Task Bounce()
        {
            if (dgvRunningSoft.SelectedCells.Count <= 0)
            {
                return;
            }
            foreach (DataGridCellInfo cell in dgvRunningSoft.SelectedCells)
            {
                DataRow row = ((DataRowView)cell.Item).Row;
                if (row.ItemArray[10] == DBNull.Value)
                {
                    continue;
                }
                string bounce = (string)row.ItemArray[10];
                if (string.IsNullOrEmpty(bounce))
                {
                    continue;
                }
                string uri = MainWindow.domain + bounce;

                await MainWindow.GET(uri, MainWindow.cookies);
            }

            LoadData();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void btnKill_Click(object sender, RoutedEventArgs e)
        {
            await Kill();
        }

        private async void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            await Complete();
        }

        private void btnAutoRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!autoRefreshOn)
            {
                int ms = Convert.ToInt32(txbAutoRefreshS.Text) * 1000;
                InstanceCaller.Start(ms);
            }

            autoRefreshOn = true;
            btnAutoRefresh.Content = autoRefreshOn ? "AutoRefresh is ON" : "AutoRefresh is OFF";
        }

        private async void btnBounce_Click(object sender, RoutedEventArgs e)
        {
            await Bounce();
        }



        //
    }
}
