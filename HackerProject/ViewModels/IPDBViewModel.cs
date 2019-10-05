using HackerProject.Models;
using HackerProject.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.ViewModels
{
    public class IPDBViewModel : BaseViewModel
    {
        private enum ACTION
        {
            CONNECT,
            BOUNCE
        }
        private IPDBModel selectedItem;
        private ObservableCollection<IPDBModel> ipList = new ObservableCollection<IPDBModel>();
        private string route;

        public ObservableCollection<IPDBModel> IPList
        {
            get
            {
                return ipList;
            }
            set
            {
                ipList = value;
                NotifyOfPropertyChange(() => IPList);
            }
        }

        public IPDBModel SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => CanConnect);
                NotifyOfPropertyChange(() => CanBounce);
            }
        }

        public string Route
        {
            get
            {
                return route;
            }
            set
            {
                route = value;
                NotifyOfPropertyChange(() => Route);
            }
        }

        #region Button IsEnable
        public bool CanConnect
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Connect))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanBounce
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Bounce) && SelectedItem.Admin.Equals("Yes"))
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        public IPDBViewModel()
        {
            ViewModelManager.IPDBViewModelInstance = this;
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
            var loadRoute = UpdateRoute();
            IPList.Clear();

            int o = 0;

            while (true)
            {
                string reqUri = "index.php?action=ip_db&a2=pub&_o=" + o;
                string responseString = await SendRequest.GET(reqUri);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);

                HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//form[@name='frm_mis']");

                HtmlNodeCollection childs = node.SelectNodes(@"//form[@name='frm_mis']/tr");

                int count = 0;

                for (int i = 0; i < childs.Count; i++)
                {
                    if (i == 0 || i == childs.Count - 1 || childs[i] == null)
                    {
                        continue;
                    }

                    string ip = childs[i].SelectSingleNode(@"./td[@class='green']/a").InnerText;
                    string name = childs[i].SelectSingleNode(@"./td[4]").InnerText.Replace("\r","").Replace("\t", "").Replace("\n","");
                    string admin = childs[i].SelectSingleNode(@"./td[5]/i").InnerText;
                    string owned = childs[i].SelectSingleNode(@"./td[6]").InnerText;
                    string connect = childs[i].SelectSingleNode(@".//tr[@class='m2']/td[1]/a").GetAttributeValue("href", "");
                    string bounce = childs[i].SelectSingleNode(@".//tr[@class='m2']/td[2]/a").GetAttributeValue("href", "");

                    IPDBModel newData = new IPDBModel()
                    {
                        IP = ip,
                        Name = name,
                        Admin = admin,
                        Owned = owned,
                        Connect = connect,
                        Bounce = bounce
                    };
                    IPList.Add(newData);
                    count++;
                }

                if (count == 0)
                {
                    break;
                }

                o += 20;
            }
            await loadRoute;
        }

        private async Task UpdateRoute()
        {
            List<string> route = new List<string>();
            string reqUri = "index.php?action=gate&a2=connect";
            string response = await SendRequest.GET(reqUri);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//td[@colspan=2]/span[@class='green']");
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    string ip = node.InnerText;
                    route.Add(ip);
                }
            }

            StringBuilder output = new StringBuilder();
            foreach (string i in route)
            {
                output.Append($" > {i}");
            }

            Route = output.ToString();
        }

        private async Task ActionTask(ACTION act)
        {
            if (SelectedItem != null)
            {
                switch (act)
                {
                    case ACTION.CONNECT:
                        await SendRequest.GET(SelectedItem.Connect);
                        break;
                    case ACTION.BOUNCE:
                        await SendRequest.GET(SelectedItem.Bounce);
                        break;
                }
                await LoadDataTask();
            }
        }

        public void BtnRefresh()
        {
            LoadData();
        }
        public async void BtnConnect()
        {
            // Do
            var actionTask = ActionTask(ACTION.CONNECT);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnBounce()
        {
            // Do
            var actionTask = ActionTask(ACTION.BOUNCE);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ViewModelManager.IPDBViewModelInstance = null;
        }


        // end
    }
}
