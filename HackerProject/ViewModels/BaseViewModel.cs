using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace HackerProject.ViewModels
{
    public class BaseViewModel : Screen
    {
        private bool isBusy;
        private string busyContent = "Waiting...";

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }
        public string BusyContent
        {
            get
            {
                return busyContent;
            }
            set
            {
                busyContent = value;
                NotifyOfPropertyChange(() => BusyContent);
            }
        }
                
        public BaseViewModel()
        {
        }
    }
}
