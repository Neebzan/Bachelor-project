using Caliburn.Micro;
using GameLauncher.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameLauncher.Views {
    /// <summary>
    /// Interaction logic for LoggingInPage.xaml
    /// </summary>
    public partial class LoggingInPage : BasePage {
        private Screen _viewModel;
        public LoggingInPage (System.Security.SecureString passwordRaw, string username) {
            InitializeComponent();
            _viewModel = new LoggingInViewModel(username, passwordRaw);
            this.DataContext = _viewModel;
            this.Loaded += LoggingInPage_Loaded;
        }

        private async void LoggingInPage_Loaded (object sender, RoutedEventArgs e) {
            await (_viewModel as LoggingInViewModel).Login();
        }
    }
}
