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

namespace HackerProject.ViewModels
{
    public class FileAndProgramViewModel : BaseViewModel
    {
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

        private FileProgramModel selectedItem;
        private ObservableCollection<FileProgramModel> fileProgramList = new ObservableCollection<FileProgramModel>();

        public ObservableCollection<FileProgramModel> FileProgramList
        {
            get
            {
                return fileProgramList;
            }
            set
            {
                fileProgramList = value;
                NotifyOfPropertyChange(() => FileProgramList);
            }
        }       

        public FileProgramModel SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => CanRun);
                NotifyOfPropertyChange(() => CanDelete);
                NotifyOfPropertyChange(() => CanHide);
                NotifyOfPropertyChange(() => CanUnhide);
                NotifyOfPropertyChange(() => CanEncrypt);
                NotifyOfPropertyChange(() => CanDecrypt);
                NotifyOfPropertyChange(() => CanPublic);
                NotifyOfPropertyChange(() => CanPrivate);
                NotifyOfPropertyChange(() => CanUpload);
                NotifyOfPropertyChange(() => CanDownload);
            }
        }

        #region Button IsEnable
        public bool CanRun
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Run))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanDelete
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Delete))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanHide
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Hide))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanUnhide
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Unhide))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanEncrypt
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Encrypt))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanDecrypt
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Decrypt))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanPublic
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Public))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanPrivate
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Private))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanUpload
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Upload))
                {
                    return true;
                }
                return false;
            }
        }

        public bool CanDownload
        {
            get
            {
                if (SelectedItem != null && !string.IsNullOrEmpty(SelectedItem.Download))
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        public FileAndProgramViewModel()
        {
            ViewModelManager.FileAndProgramViewModelInstance = this;
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
            FileProgramList.Clear();

            string reqUri = "index.php?action=gate&a2=files";
            string responseString = await SendRequest.GET(reqUri);

            if (string.IsNullOrEmpty(responseString))
            {
                Console.WriteLine("Load program: null");
                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(responseString);

            HtmlNode node = doc.DocumentNode.SelectSingleNode(@"//form[@name='frm_files']/table");

            HtmlNodeCollection childs = node.ChildNodes;
            string[] stringSeparators = new string[] { "'" };


            foreach (HtmlNode n in childs)
            {
                if (n.Name.Equals("script") && n.InnerText.Contains("document.write"))
                {
                    string text = n.InnerText;
                    string[] splits = text.Split(stringSeparators, StringSplitOptions.None);

                    string encoded = splits[1];
                    string decoded = WebUtility.UrlDecode(encoded);

                    AddProgram(decoded);
                }
            }
        }

        private void AddProgram(string decoded)
        {
            try
            {
                FileProgramModel p = new FileProgramModel();

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
                        p.Public = href;
                    }
                    else if (href.Contains("Private"))
                    {
                        p.Private = href;
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

                FileProgramList.Add(p);
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
                    case ACTION.RUN:
                        await SendRequest.GET(SelectedItem.Run);
                        break;
                    case ACTION.DELETE:
                        await SendRequest.GET(SelectedItem.Delete);
                        break;
                    case ACTION.HIDE:
                        await SendRequest.GET(SelectedItem.Hide);
                        break;
                    case ACTION.UNHIDE:
                        await SendRequest.GET(SelectedItem.Unhide);
                        break;
                    case ACTION.ENCRYPT:
                        await SendRequest.GET(SelectedItem.Encrypt);
                        break;
                    case ACTION.DECRYPT:
                        await SendRequest.GET(SelectedItem.Decrypt);
                        break;
                    case ACTION.PUBLIC:
                        await SendRequest.GET(SelectedItem.Public);
                        break;
                    case ACTION.PRIVATE:
                        await SendRequest.GET(SelectedItem.Private);
                        break;
                    case ACTION.UPLOAD:
                        await SendRequest.GET(SelectedItem.Upload);
                        break;
                    case ACTION.DOWNLOAD:
                        await SendRequest.GET(SelectedItem.Download);
                        break;
                }
                ViewModelManager.RunningProcessViewModelInstance.LoadData();
            }
        }

        public void BtnRefresh()
        {
            LoadData();
        }

        public async void BtnRun()
        {
            // Do
            var actionTask = ActionTask(ACTION.RUN);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnDelete()
        {
            // Do
            var actionTask = ActionTask(ACTION.DELETE);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnHide()
        {
            // Do
            var actionTask = ActionTask(ACTION.HIDE);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnUnhide()
        {
            // Do
            var actionTask = ActionTask(ACTION.UNHIDE);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnEncrypt()
        {
            // Do
            var actionTask = ActionTask(ACTION.ENCRYPT);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnDecrypt()
        {
            // Do
            var actionTask = ActionTask(ACTION.DECRYPT);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnPublic()
        {
            // Do
            var actionTask = ActionTask(ACTION.PUBLIC);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnPrivate()
        {
            // Do
            var actionTask = ActionTask(ACTION.PRIVATE);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnUpload()
        {
            // Do
            var actionTask = ActionTask(ACTION.UPLOAD);

            // Start waiting
            IsBusy = true;

            // Wait
            await actionTask;

            // End waiting
            IsBusy = false;
        }
        public async void BtnDownload()
        {
            // Do
            var actionTask = ActionTask(ACTION.DOWNLOAD);

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
            ViewModelManager.FileAndProgramViewModelInstance = null;
        }


        // end
    }
}
