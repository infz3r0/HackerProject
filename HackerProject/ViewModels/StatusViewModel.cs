using HackerProject.Models;
using HackerProject.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.ViewModels
{
    public class StatusViewModel : BaseViewModel
    {
        private StatusModel status = new StatusModel();

        public string CurCPU
        {
            get
            {
                return status.CurCPU;
            }
            set
            {
                status.CurCPU = value;
                NotifyOfPropertyChange(() => CurCPU);
            }
        }

        public string MaxCPU
        {
            get
            {
                return status.MaxCPU;
            }
            set
            {
                status.MaxCPU = value;
                NotifyOfPropertyChange(() => MaxCPU);
            }
        }

        public string PerCPU
        {
            get
            {
                return $" ({status.PerCPU})";
            }
            set
            {
                status.PerCPU = value;
                NotifyOfPropertyChange(() => PerCPU);
            }
        }

        public string CurRAM
        {
            get
            {
                return status.CurRAM;
            }
            set
            {
                status.CurRAM = value;
                NotifyOfPropertyChange(() => CurRAM);
            }
        }

        public string MaxRAM
        {
            get
            {
                return status.MaxRAM;
            }
            set
            {
                status.MaxRAM = value;
                NotifyOfPropertyChange(() => MaxRAM);
            }
        }

        public string PerRAM
        {
            get
            {
                return $" ({status.PerRAM})";
            }
            set
            {
                status.PerRAM = value;
                NotifyOfPropertyChange(() => PerRAM);
            }
        }

        public string CurBandwidth
        {
            get
            {
                return status.CurBandwidth;
            }
            set
            {
                status.CurBandwidth = value;
                NotifyOfPropertyChange(() => CurBandwidth);
            }
        }

        public string MaxBandwidth
        {
            get
            {
                return status.MaxBandwidth;
            }
            set
            {
                status.MaxBandwidth = value;
                NotifyOfPropertyChange(() => MaxBandwidth);
            }
        }

        public string PerBandwidth
        {
            get
            {
                return $" ({status.PerBandwidth})";
            }
            set
            {
                status.PerBandwidth = value;
                NotifyOfPropertyChange(() => PerBandwidth);
            }
        }

        public string CurHDD
        {
            get
            {
                return status.CurHDD;
            }
            set
            {
                status.CurHDD = value;
                NotifyOfPropertyChange(() => CurHDD);
            }
        }

        public string MaxHDD
        {
            get
            {
                return status.MaxHDD;
            }
            set
            {
                status.MaxHDD = value;
                NotifyOfPropertyChange(() => MaxHDD);
            }
        }

        public string PerHDD
        {
            get
            {
                return $" ({status.PerHDD})";
            }
            set
            {
                status.PerHDD = value;
                NotifyOfPropertyChange(() => PerHDD);
            }
        }

        private CustomTimer autoRefreshTimer;
        private double autoRefreshInterval;
        private string autoRefreshContent;
        
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

        public StatusViewModel()
        {
            ViewModelManager.StatusViewModelInstance = this;
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
            string reqUri = "index.php?action=gate&a2=run";
            string responseString = await SendRequest.GET(reqUri);

            var doc = new HtmlDocument();
            doc.LoadHtml(responseString);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='g']");

            CurCPU = nodes[0].InnerText;
            MaxCPU = nodes[1].InnerText;
            CurRAM = nodes[2].InnerText;
            MaxRAM = nodes[3].InnerText;
            CurBandwidth = nodes[4].InnerText;
            MaxBandwidth = nodes[5].InnerText;

            nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='p']");

            PerCPU = nodes[0].InnerText;
            PerRAM = nodes[1].InnerText;
            PerBandwidth = nodes[2].InnerText;

            string reqUri2 = "index.php?action=gate&a2=files";
            string responseString2 = await SendRequest.GET(reqUri2);

            doc = new HtmlDocument();
            doc.LoadHtml(responseString2);

            nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='g']");

            CurHDD = nodes[0].InnerText;
            MaxHDD = nodes[1].InnerText;

            nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan='6']/span[@class='p']");

            PerHDD = nodes[0].InnerText;
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
            ViewModelManager.StatusViewModelInstance = null;
        }


        // end
    }
}
