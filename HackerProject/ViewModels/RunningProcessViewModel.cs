using HackerProject.Models;
using HackerProject.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace HackerProject.ViewModels
{
    public class RunningProcessViewModel : BaseViewModel
    {
        private enum ACTION
        {
            KILL,
            COMPLETE
        }

        private CustomTimer autoRefreshTimer;
        private double autoRefreshInterval;
        private string autoRefreshContent;
        private RunningProcessModel selectedItem;
        private ObservableCollection<RunningProcessModel> runningProcessList = new ObservableCollection<RunningProcessModel>();

        public ObservableCollection<RunningProcessModel> RunningProcessList
        {
            get
            {
                return runningProcessList;
            }
            set
            {
                runningProcessList = value;
                NotifyOfPropertyChange(() => RunningProcessList);
            }
        }

        public RunningProcessModel SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => CanKill);
                NotifyOfPropertyChange(() => CanComplete);
            }
        }

        public double AutoRefreshInterval
        {
            get
            {
                return autoRefreshInterval;
            }
            set
            {
                if (value <= 0)
                {
                    value = 1;
                }
                autoRefreshInterval = value;
                NotifyOfPropertyChange(() => AutoRefreshInterval);
            }
        }

        public string AutoRefreshContent
        {
            get
            {
                return autoRefreshContent;
            }
            set
            {
                autoRefreshContent = value;
                NotifyOfPropertyChange(() => AutoRefreshContent);
            }
        }

        #region Button IsEnable
        public bool CanKill
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Kill))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanComplete
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Complete))
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        public RunningProcessViewModel()
        {            
            ViewModelManager.RunningProcessViewModelInstance = this;
            AutoRefreshContent = "Auto: OFF";
            AutoRefreshInterval = 1;
            LoadData();
        }

        public async void LoadData()
        {
            // Do
            var loadDataTask = LoadDataTask();

            // Start waiting
            IsBusy = true;

            // Wait
            await loadDataTask;

            // End waiting
            IsBusy = false;
        }

        private async Task LoadDataTask()
        {
            RunningProcessList.Clear();

            int o = 0;

            while (true)
            {
                string reqUri = "index.php?action=gate&a2=run&_o=" + o;
                string responseString = await SendRequest.GET(reqUri);

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

                        AddProcess(decoded);

                        count++;
                    }
                }

                if (count == 0)
                {
                    break;
                }

                o += 50;
            }
        }

        private void AddProcess(string decoded)
        {
            try
            {
                RunningProcessModel p = new RunningProcessModel();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(decoded);

                HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//tr/td[@class='snl']");
                p.Id = node.InnerText;

                if (!string.IsNullOrEmpty(p.Id))
                {
                    p.Kill = "index.php?action=gate&a2=run&k_pid=" + p.Id;
                }

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

                RunningProcessList.Add(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task ActionTask(ACTION act)
        {
            if (SelectedItem != null)
            {
                switch (act)
                {
                    case ACTION.KILL:
                        await SendRequest.GET(SelectedItem.Kill);
                        break;
                    case ACTION.COMPLETE:
                        await SendRequest.GET(SelectedItem.Complete);
                        break;
                }
                await LoadDataTask();
            }
        }

        public void BtnRefresh()
        {
            LoadData();
        }
        public async void BtnKill()
        {
            // Do
            var actionTask = ActionTask(ACTION.KILL);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnComplete()
        {
            // Do
            var actionTask = ActionTask(ACTION.COMPLETE);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public void BtnAutoRefresh()
        {
            if (autoRefreshTimer == null)
            {
                autoRefreshTimer = new CustomTimer(() => LoadData(), AutoRefreshInterval * 1000);
                AutoRefreshContent = "Auto: ON";
            }
            else
            {
                autoRefreshTimer.Timer.Close();
                autoRefreshTimer = null;
                AutoRefreshContent = "Auto: OFF";
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ViewModelManager.RunningProcessViewModelInstance = null;
        }


        // end
    }
}
