using HackerProject.Models;
using HackerProject.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.ViewModels
{
    public class LogViewModel : BaseViewModel
    {
        private CustomTimer autoRefreshTimer;
        private double autoRefreshInterval;
        private string autoRefreshContent;
        private LogModel selectedItem;
        private ObservableCollection<LogModel> logList = new ObservableCollection<LogModel>();

        public ObservableCollection<LogModel> LogList
        {
            get
            {
                return logList;
            }
            set
            {
                logList = value;
                NotifyOfPropertyChange(() => LogList);
            }
        }

        public LogModel SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
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

        public LogViewModel()
        {
            ViewModelManager.LogViewModelInstance = this;
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
            LogList.Clear();

            int o = 0;
            int i = -1;

            while (true)
            {
                if (i >= 50)
                {
                    break;
                }

                string reqUri = "index.php?action=gate&a2=logs&_o=" + o;
                string responseString = await SendRequest.GET(reqUri);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//td[@class='dbg' and @valign='top']");

                if (nodes == null)
                {
                    break;
                }

                foreach (HtmlNode n in nodes)
                {
                    LogModel l = new LogModel();
                    l.Time = n.InnerText;
                    LogList.Add(l);
                }

                HtmlNodeCollection nodes2 = doc.DocumentNode.SelectNodes(@"//textarea[@name='log_str']");


                foreach (HtmlNode n2 in nodes2)
                {
                    i++;
                    string s = n2.InnerText;
                    LogList[i].Logs = s;
                    if (s.Contains("[localhost]") || s.Contains("Origin proxy") || s.Contains("Kernel"))
                    {
                        LogList[i].Alert = 0;
                    }
                    else if (s.Contains("Download"))
                    {
                        LogList[i].Alert = 1;
                    }
                    else
                    {
                        LogList[i].Alert = 2;
                    }
                }

                o += 10;

            } //end while
        }
        
        public void BtnRefresh()
        {
            LoadData();
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
            ViewModelManager.LogViewModelInstance = null;
        }


        // end
    }
}
