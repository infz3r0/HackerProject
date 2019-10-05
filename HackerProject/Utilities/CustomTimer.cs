using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace HackerProject.Utilities
{
    public class CustomTimer
    {
        public Action Action { get; set; }
        public Timer Timer { get; set; }

        public CustomTimer()
        {
            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(Action);
        }

        public CustomTimer(Action action, double interval)
        {            
            Action = action;
            Timer = new Timer();
            Timer.Elapsed += Timer_Elapsed;
            Timer.Interval = 1000;
            Timer.AutoReset = true;
            Timer.Interval = interval;
            Timer.Start();
        }

    }
}
