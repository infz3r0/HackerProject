using HackerProject.Models;
using HackerProject.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HackerProject.ViewModels
{
    public class MissionViewModel : BaseViewModel
    {
        private enum ACTION
        {
            MARK,
            COMPLETE
        }

        private MissionModel selectedItem;
        private ObservableCollection<MissionModel> missionList = new ObservableCollection<MissionModel>();
        private ObservableCollection<MissionModel> markedList = new ObservableCollection<MissionModel>();
        public ObservableCollection<MissionModel> MissionList
        {
            get
            {
                return missionList;
            }
            set
            {
                missionList = value;
                NotifyOfPropertyChange(() => MissionList);
            }
        }

        public ObservableCollection<MissionModel> MarkedList
        {
            get
            {
                return markedList;
            }
            set
            {
                markedList = value;
                NotifyOfPropertyChange(() => MarkedList);
            }
        }

        public MissionModel SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => CanMark);
                NotifyOfPropertyChange(() => CanComplete);
            }
        }

        #region Button IsEnable
        public bool CanMark
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Mark))
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

        public MissionViewModel()
        {
            ViewModelManager.MissionViewModelInstance = this;
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
            var missionTask = LoadMissionTask("index.php?action=mission&stype=0&tid=0&mtype=0&order=&_o=");
            var markedTask = LoadMarkedTask("index.php?action=mission&stype=3&tid=0&mtype=0&order=&_o=");
            await missionTask;
            await markedTask;
        }

        private async Task LoadMissionTask(string uri)
        {
            MissionList.Clear();
            int o = 0;
            while (true)
            {
                string reqUri = uri + o;
                string responseString = await SendRequest.GET(reqUri);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//form[@name='frm_mis']/tr");

                int count = 0;

                Regex regex = new Regex(@"# \d+");
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (i == 0 || i == nodes.Count - 1)
                    {
                        continue;
                    }

                    string complete = nodes[i].SelectSingleNode(@"./td[1]/table/tr[@class='m2']//a").GetAttributeValue("href", "");
                    string mark = nodes[i].SelectSingleNode(@"./td[1]/table/tr[2]//a").GetAttributeValue("href", "");
                    string targetIP = nodes[i].SelectSingleNode(@"./td[2]/table/tr[1]//a/span[@class='green']").InnerText;
                    string type = nodes[i].SelectSingleNode(@"./td[3]/a/span").InnerText;
                    string detail = StringHelper.RemoveSpecial(nodes[i].SelectSingleNode(@"./td[4]//td[@align='justify']").InnerText);
                    string reward = nodes[i].SelectSingleNode(@"./td[5]").InnerText;
                    string fileID = "";
                    var match = regex.Match(detail);
                    if (match.Success)
                    {
                        fileID = match.Value.Replace("#", "").Trim();
                    }

                    MissionModel newData = new MissionModel()
                    {
                        Complete = complete,
                        Mark = mark,
                        TargetIP = targetIP,
                        Type = type,
                        Details = detail,
                        Reward = reward,
                        FileID = fileID
                    };
                    MissionList.Add(newData);

                    count++;
                }

                if (count == 0)
                {
                    break;
                }

                o += 50;
            }
        }

        private async Task LoadMarkedTask(string uri)
        {
            MarkedList.Clear();
            int o = 0;
            while (true)
            {
                string reqUri = uri + o;
                string responseString = await SendRequest.GET(reqUri);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(responseString);

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"//form[@name='frm_mis']/tr");

                int count = 0;

                Regex regex = new Regex(@"# \d+");
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (i == 0 || i == nodes.Count - 1)
                    {
                        continue;
                    }

                    string complete = nodes[i].SelectSingleNode(@"./td[1]/table/tr[@class='m2']//a").GetAttributeValue("href", "");
                    string mark = ""; // nodes[i].SelectSingleNode(@"./td[1]/table/tr[2]//a").GetAttributeValue("href", "");
                    string targetIP = nodes[i].SelectSingleNode(@"./td[2]/table/tr[1]//a/span[@class='green']").InnerText;
                    string type = nodes[i].SelectSingleNode(@"./td[3]/a/span").InnerText;
                    string detail = StringHelper.RemoveSpecial(nodes[i].SelectSingleNode(@"./td[4]//td[@align='justify']").InnerText);
                    string reward = nodes[i].SelectSingleNode(@"./td[5]").InnerText;
                    string fileID = "";
                    var match = regex.Match(detail);
                    if (match.Success)
                    {
                        fileID = match.Value.Replace("#", "").Trim();
                    }

                    MissionModel newData = new MissionModel()
                    {
                        Complete = complete,
                        Mark = mark,
                        TargetIP = targetIP,
                        Type = type,
                        Details = detail,
                        Reward = reward,
                        FileID = fileID
                    };
                    MarkedList.Add(newData);

                    count++;
                }

                if (count == 0)
                {
                    break;
                }

                o += 50;
            }
        }

        private async Task ActionTask(ACTION act)
        {
            if (SelectedItem != null)
            {
                switch (act)
                {
                    case ACTION.MARK:
                        await SendRequest.GET(SelectedItem.Mark);
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
        public async void BtnMark()
        {
            // Do
            var actionTask = ActionTask(ACTION.MARK);

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

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            ViewModelManager.MissionViewModelInstance = null;
        }


        // end
    }
}
