using Caliburn.Micro;
using HackerProject.Models;
using HackerProject.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HackerProject.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string username;
        private string password;
        private string sessionId;
        private bool canLogin;
        private bool canLogout;

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }
        public string SessionId
        {
            get
            {
                return sessionId;
            }
            set
            {
                sessionId = value;
                NotifyOfPropertyChange(() => SessionId);
            }
        }
        public bool CanLogin
        {
            get
            {
                return canLogin;
            }
            set
            {
                canLogin = value;
                NotifyOfPropertyChange(() => CanLogin);
            }
        }
        public bool CanLogout
        {
            get
            {
                return canLogout;
            }
            set
            {
                canLogout = value;
                NotifyOfPropertyChange(() => CanLogout);
            }
        }
        
        public MainViewModel()
        {
            ViewModelManager.MainViewModelInstance = this;
            SessionId = "Offline";
            SetLoginState(false);
        }

        private void SetLoginState(bool loggedIn)
        {
            CanLogin = !loggedIn;
            CanLogout = loggedIn;
        }
        private void SetInputsState(bool isEnabled)
        {
            CanLogin = isEnabled;
            CanLogout = isEnabled;
        }

        private async Task<bool> LoginTask(string username, string password)
        {
            var values = new Dictionary<string, string>();
            values.Add("user", username);
            values.Add("pwd", password); 
            values.Add("action", "login");

            string reqUri = "index.php";
            string responseString = await SendRequest.POST(reqUri, values);

            if (string.IsNullOrEmpty(responseString))
            {
                SessionId = "Login error";
                return false;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(responseString);
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//form[@name='frm_gate']");

            if (node != null)
            {
                // Login success
                // Get sessionId
                var cookies = Client.CookieContainer.GetCookies(Client.GetUri("index.php"));
                var sessionId = cookies["PHPSESSID"];
                SessionId = "Online";



                //Task tsk = LoadStatus();

                //OpenWindow_FilesAndPrograms();
                //OpenWindow_RunningSoftware();
                //OpenWindow_Logs();
                //await UpdateRoute();

                //await tsk;

                //Thread t = new Thread(AutoRefreshStatus);
                //t.SetApartmentState(ApartmentState.STA);
                //t.IsBackground = true;
                //t.Start();
                return true;
            }
            else
            {
                // Login fail
                SessionId = "Login fail";
                return false;
            }
        }

        private async Task<bool> LogoutTask()
        {
            string reqUri = "index.php?action=logout";
            string responseString = await SendRequest.GET(reqUri);

            if (string.IsNullOrEmpty(responseString))
            {
                SessionId = "Logout error";
                return false;
            }

            SessionId = "Offline";
            return true;
        }

        public async void Login()
        {
            // Do login
            var loginTask = LoginTask(Username, Password);

            // Start waiting
            IsBusy = true;

            // Disable all input
            SetInputsState(false);

            // Wait login
            bool loginSuccess = await loginTask;

            if (loginSuccess)
            {
                SetLoginState(true);
            }
            else
            {
                SetLoginState(false);
            }

            // End waiting
            IsBusy = false;
            BusyContent = string.Empty;
        }

        public async void Logout()
        {
            // Do login
            var logoutTask = LogoutTask();

            // Start waiting
            IsBusy = true;

            // Disable all input
            SetInputsState(false);            

            // Wait login
            bool logoutSuccess = await logoutTask;

            if (logoutSuccess)
            {
                SetLoginState(false);
            }
            else
            {
                SetLoginState(true);
            }

            // End waiting
            IsBusy = false;
            BusyContent = string.Empty;
        }

        public void BtnProgram()
        {
            if (ViewModelManager.FileAndProgramViewModelInstance == null)
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowWindow(new FileAndProgramViewModel());
            }
        }

        public void BtnRunning()
        {
            if (ViewModelManager.RunningProcessViewModelInstance == null)
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowWindow(new RunningProcessViewModel());
            }
        }

        public void BtnLog()
        {
            if (ViewModelManager.LogViewModelInstance == null)
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowWindow(new LogViewModel());
            }
        }

        public void BtnIPDB()
        {
            if (ViewModelManager.IPDBViewModelInstance == null)
            {
                WindowManager windowManager = new WindowManager();
                windowManager.ShowWindow(new IPDBViewModel());
            }
        }
    }
}
