using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data;
using HtmlAgilityPack;
using System.Threading;
using System.Windows.Threading;

namespace HackerProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static HttpClient client;
        public static CookieContainer cookies = new CookieContainer();
        public static string domain = "http://www.hacker-project.com/";
        public static List<string> routes;

        public static Thread InstanceCaller;
        private bool autoRefreshOn = false;

        public MainWindow()
        {
            InitializeComponent();

            SetIsEnableFunc(false);
            btnLogin.IsEnabled = true;

            InstanceCaller = new Thread(new ParameterizedThreadStart(AutoRefresh));
        }

        private void SetIsEnableFunc(bool isEnable)
        {
            btnLogin.IsEnabled = isEnable;
            btnRunningSoft.IsEnabled = isEnable;
            btnLogout.IsEnabled = isEnable;
            btnRoute.IsEnabled = isEnable;
            btnProgram.IsEnabled = isEnable;
        }

        private async Task<string> POST(string uri, Dictionary<string, string> param, CookieContainer cookies)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            var values = param;

            client = new HttpClient(handler);

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync(uri, content);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public static async Task<string> GET(string uri, CookieContainer cookies)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            client = new HttpClient(handler);

            var response = await client.GetAsync(uri);

            //will throw an exception if not successful
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private async Task Login(string username, string password)
        {
            SetIsEnableFunc(false);

            cookies = new CookieContainer();

            var values = new Dictionary<string, string>();
            values.Add("user", username);
            values.Add("pwd", password);
            values.Add("action", "login");

            string reqUri = domain + "index.php";

            string responseString = await POST(reqUri, values, cookies);

            // WriteAllText creates a file, writes the specified string to the file,
            // and then closes the file.    You do NOT need to call Flush() or Close().
            //System.IO.File.WriteAllText(@"temp.html", responseString);

            //System.Diagnostics.Process.Start(@"temp.html");

            var doc = new HtmlDocument();
            doc.LoadHtml(responseString);

            HtmlNode node = doc.DocumentNode.SelectSingleNode("//form[@name='frm_gate']");
            if (node != null)
            {
                Uri uri = new Uri(domain + "index.php");
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
                foreach (Cookie cookie in responseCookies)
                {
                    if (cookie.Name.Equals("PHPSESSID"))
                    {
                        Console.WriteLine("PHPSESSID:" + cookie.Value);
                        txbID.Text = cookie.Value;
                        Task tsk = LoadStatus();

                        OpenWindow_FilesAndPrograms();
                        OpenWindow_RunningSoftware();                        
                        await UpdateRoute();

                        await tsk;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Login failed");
            }

            SetIsEnableFunc(true);
        }

        public async void AutoRefresh(object oms)
        {
            int ms = (int)oms;
            while (true)
            {
                await this.Dispatcher.Invoke(() => this.LoadStatus(), DispatcherPriority.Normal);


                Thread.Sleep(ms);
            }
        }

        private async Task LoadStatus()
        {
            string reqUri = domain + "index.php?action=gate&a2=run";
            string responseString = await GET(reqUri, cookies);

            string reqUri2 = domain + "index.php?action=gate&a2=files";
            string responseString2 = await GET(reqUri2, cookies);

            var doc = new HtmlDocument();
            doc.LoadHtml(responseString);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='g']");

            string CPU = nodes[0].InnerText + "/" + nodes[1].InnerText;
            string RAM = nodes[2].InnerText + "/" + nodes[3].InnerText;
            string BandW = nodes[4].InnerText + "/" + nodes[5].InnerText;

            nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='p']");

            string CPUp = nodes[0].InnerText;
            string RAMp = nodes[1].InnerText;
            string BandWp = nodes[2].InnerText;

            txkCPU_Value.Text = CPU + " | " + CPUp;
            txkRAM_Value.Text = RAM + " | " + RAMp;
            txkBandW_Value.Text = BandW + " | " + BandWp;

            doc = new HtmlDocument();
            doc.LoadHtml(responseString2);

            nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='g']");

            string HDD = nodes[0].InnerText + "/" + nodes[1].InnerText;

            nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='p']");

            string HDDp = nodes[0].InnerText;

            txkHDD_Value.Text = HDD + " | " + HDDp;
        }

        private async Task UpdateRoute()
        {
            List<string> route = new List<string>();
            string reqUri = domain + "index.php?action=gate&a2=connect";
            string response = await GET(reqUri, cookies);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='2']/span[@class='green']");
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    string ip = node.InnerText;
                    route.Add(ip);
                }
            }

            routes = route;

            string output = "Route: ";
            foreach (string i in routes)
            {
                output += i + " -> ";
            }

            txkRoute.Text = output;
        }

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txbUsename.Text;
            string password = txbPassword.Password;
            await Login(username, password);
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            SetIsEnableFunc(false);
            string reqUri = domain + "index.php?action=logout";
            await GET(reqUri, cookies);
            SetIsEnableFunc(true);
        }

        private void OpenWindow_RunningSoftware()
        {
            if (IsWindowOpen<Window>("RunningSoftware"))
            {
                foreach (Window wd in Application.Current.Windows)
                {
                    if (wd.Name.Equals("RunningSoftware"))
                    {
                        wd.Close();
                    }
                }
            }
            RunningSoftware w = new RunningSoftware();
            w.Name = "RunningSoftware";
            w.Show();
        }

        private void btnRunningSoft_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow_RunningSoftware();
        }

        private async void btnRoute_Click(object sender, RoutedEventArgs e)
        {
            await UpdateRoute();
        }

        private void OpenWindow_FilesAndPrograms()
        {
            if (IsWindowOpen<Window>("FilesAndPrograms"))
            {
                foreach (Window wd in Application.Current.Windows)
                {
                    if (wd.Name.Equals("FilesAndPrograms"))
                    {
                        wd.Close();
                    }
                }
            }
            FilesAndPrograms w = new FilesAndPrograms();
            w.Name = "FilesAndPrograms";
            w.Show();
        }

        private void btnProgram_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow_FilesAndPrograms();
        }

        private void btnRefreshStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!autoRefreshOn)
            {
                InstanceCaller.Start(10000);
            }

            autoRefreshOn = true;
        }

        private void OpenWindow_Logs()
        {
            if (IsWindowOpen<Window>("Logs"))
            {
                foreach (Window wd in Application.Current.Windows)
                {
                    if (wd.Name.Equals("Logs"))
                    {
                        wd.Close();
                    }
                }
            }
            Logs w = new Logs();
            w.Name = "Logs";
            w.Show();
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow_Logs();
        }


        //
    }
}
