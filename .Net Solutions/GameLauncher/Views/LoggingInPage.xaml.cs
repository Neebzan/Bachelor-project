using Caliburn.Micro;
using GameLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
using System.Windows.Threading;

namespace GameLauncher.Views {
    /// <summary>
    /// Interaction logic for LoggingInPage.xaml
    /// </summary>
    public partial class LoggingInPage : BasePage {
        private Screen _viewModel;
        private bool _useToken;
        private string _username;
        private SecureString _password;

        public LoggingInPage (string username, SecureString password) {
            _useToken = false;
            _username = username;
            _password = password;

            InitializeComponent();
            _viewModel = new LoggingInViewModel();
            this.DataContext = _viewModel;
            this.Loaded += LoggingInPage_Loaded;
        }

        public LoggingInPage () {
            _useToken = true;

            InitializeComponent();
            _viewModel = new LoggingInViewModel();
            this.DataContext = _viewModel;
            this.Loaded += LoggingInPage_Loaded;
        }

        private async void LoggingInPage_Loaded (object sender, RoutedEventArgs e) {
            bool success = false;

            if (_useToken)
                success = await (_viewModel as LoggingInViewModel).LoginTokenAsync();
            else
                success = await (_viewModel as LoggingInViewModel).LoginAsync(_username, _password);


            if (success) {
                Dispatcher.Invoke(DispatcherPriority.Background,
                    new Action(async () => {
                        await AnimateOut();
                        (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new UserPage());
                    }));
            }
            else {
                Dispatcher.Invoke(DispatcherPriority.Background,
                    new Action(async () => {
                        await AnimateOut();
                        (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new LoginPage());
                    }));
            }
        }
    }
}
