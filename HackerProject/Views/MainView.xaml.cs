using HackerProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HackerProject.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            MainViewModel viewModel = (MainViewModel)this.DataContext;
            viewModel.Password = Password.Password;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Username.Focus();
            Username.Text = "infz3r0";
            Password.Password = "Falcon1412";
        }
    }
}
