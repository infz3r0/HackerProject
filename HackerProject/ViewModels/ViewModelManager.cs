using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.ViewModels
{
    public static class ViewModelManager
    {
        public static MainViewModel MainViewModelInstance { get; set; }
        public static FileAndProgramViewModel FileAndProgramViewModelInstance { get; set; }
        public static RunningProcessViewModel RunningProcessViewModelInstance { get; set; }
        public static StatusViewModel StatusViewModelInstance { get; set; }
        public static LogViewModel LogViewModelInstance { get; set; }
        public static IPDBViewModel IPDBViewModelInstance { get; set; }
        public static MissionViewModel MissionViewModelInstance { get; set; }

    }
}
