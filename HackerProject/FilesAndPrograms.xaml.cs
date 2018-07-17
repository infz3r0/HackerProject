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
    /// Interaction logic for FilesAndPrograms.xaml
    /// </summary>
    public partial class FilesAndPrograms : Window
    {
        public DataTable data = new DataTable();
        public List<Program> programs = new List<Program>();
        public string domain = MainWindow.domain;

        private enum ACTION
        {
            RUN,
            DELETE,
            HIDE,
            UNHIDE,
            ENCRYPT,
            DECRYPT,
            PUBLIC,
            PRIVATE,
            UPLOAD,
            DOWNLOAD
        }

        public FilesAndPrograms()
        {
            InitializeComponent();

            LoadData();
        }

        private void AddProgram(string decoded)
        {
            try
            {
                Program p = new Program();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(decoded);

                HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='sni']");
                p.Id = node.InnerText;

                node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='p1']");
                p.Type = node.InnerText;

                node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='sm2']");
                p.Version = Convert.ToDouble(node.InnerText);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//tr/td[@class='sm']");
                p.Name = nodes[0].InnerText;
                string size = nodes[1].InnerText;
                string[] seperate = { " " };
                string[] splits = size.Split(seperate, StringSplitOptions.None);

                switch (splits[1])
                {
                    case "Mb":
                        p.Size = Convert.ToInt64(Convert.ToDouble(splits[0]) * 1024);
                        break;
                    case "Gb":
                        p.Size = Convert.ToInt64(Convert.ToDouble(splits[0]) * 1024 * 1024);
                        break;
                }

                nodes = doc.DocumentNode.SelectNodes(@"//a");
                foreach (HtmlNode n in nodes)
                {
                    string href = n.Attributes[0].Value;
                    if (href.Contains("run"))
                    {
                        p.Run = href;
                    }
                    else if (href.Contains("delete"))
                    {
                        p.Delete = href;
                    }
                    else if (href.Contains("unhide"))
                    {
                        p.Unhide = href;
                    }
                    else if (href.Contains("hide"))
                    {
                        p.Hide = href;
                    }
                    else if (href.Contains("encrypt"))
                    {
                        p.Encrypt = href;
                    }
                    else if (href.Contains("decrypt"))
                    {
                        p.Decrypt = href;
                    }
                    else if (href.Contains("Public"))
                    {
                        p.Spublic = href;
                    }
                    else if (href.Contains("Private"))
                    {
                        p.Sprivate = href;
                    }
                    else if (href.Contains("upload"))
                    {
                        p.Upload = href;
                    }
                    else if (href.Contains("download"))
                    {
                        p.Download = href;
                    }
                }

                programs.Add(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task ListPrograms()
        {
            programs.Clear();

            string reqUri = MainWindow.domain + "index.php?action=gate&a2=files";
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

                    AddProgram(decoded);

                    count++;
                }
            }
            
        }

        private async void LoadData()
        {
            pgbLoad.Value = 0;
            data = await GetData();
            dgvFilesPrograms.DataContext = data.DefaultView;
            pgbLoad.Value = 100;
        }

        private async Task<DataTable> GetData()
        {
            await ListPrograms();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("Type");
            dt.Columns.Add("Name");
            dt.Columns.Add("Version");
            dt.Columns.Add("Size");

            dt.Columns.Add("Run");
            dt.Columns.Add("Delete");
            dt.Columns.Add("Hide");
            dt.Columns.Add("Unhide");
            dt.Columns.Add("Encrypt");
            dt.Columns.Add("Decrypt");
            dt.Columns.Add("Public");
            dt.Columns.Add("Private");
            dt.Columns.Add("Upload");
            dt.Columns.Add("Download");
            foreach (Program p in programs)
            {
                DataRow row = dt.NewRow();
                row["Id"] = p.Id;
                row["Type"] = p.Type;
                row["Name"] = p.Name;
                row["Version"] = p.Version;
                row["Size"] = p.Size;
                row["Run"] = p.Run;
                row["Delete"] = p.Delete;
                row["Hide"] = p.Hide;
                row["Unhide"] = p.Unhide;
                row["Encrypt"] = p.Encrypt;
                row["Decrypt"] = p.Decrypt;
                row["Public"] = p.Spublic;
                row["Private"] = p.Sprivate;
                row["Upload"] = p.Upload;
                row["Download"] = p.Download;

                dt.Rows.Add(row);
            }
            

            return dt;
        }

        private async Task Action(ACTION act)
        {
            if (dgvFilesPrograms.SelectedCells.Count > 0)
            {
                //
                int index = -1;
                switch (act)
                {
                    case ACTION.RUN:
                        index = 5;
                        break;
                    case ACTION.DELETE:
                        index = 6;
                        break;
                    case ACTION.HIDE:
                        index = 7;
                        break;
                    case ACTION.UNHIDE:
                        index = 8;
                        break;
                    case ACTION.ENCRYPT:
                        index = 9;
                        break;
                    case ACTION.DECRYPT:
                        index = 10;
                        break;
                    case ACTION.PUBLIC:
                        index = 11;
                        break;
                    case ACTION.PRIVATE:
                        index = 12;
                        break;
                    case ACTION.UPLOAD:
                        index = 13;
                        break;
                    case ACTION.DOWNLOAD:
                        index = 14;
                        break;
                }
                //
                foreach (DataGridCellInfo cell in dgvFilesPrograms.SelectedCells)
                {
                    DataRow row = ((DataRowView)cell.Item).Row;
                    if (row.ItemArray[index] == DBNull.Value)
                    {
                        continue;
                    }
                    string uri = (string)row.ItemArray[index];
                    if (string.IsNullOrEmpty(uri))
                    {
                        continue;
                    }

                    await MainWindow.GET(domain + uri, MainWindow.cookies);
                }

                Application.Current.Windows.OfType<RunningSoftware>().First().LoadData();
            }
        }

        #region

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.RUN);
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.DELETE);
        }

        private async void btnHide_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.HIDE);
        }

        private async void btnUnhide_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.UNHIDE);
        }

        private async void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.ENCRYPT);
        }

        private async void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.DECRYPT);
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.UPLOAD);
        }

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.DOWNLOAD);
        }

        private async void btnPublic_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.PUBLIC);
        }

        private async void btnPrivate_Click(object sender, RoutedEventArgs e)
        {
            await Action(ACTION.PRIVATE);
        }

        #endregion

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }



        //
    }
}
