using Caliburn.Micro;
using GameLauncher.Properties;
using GameLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
using System.Windows.Threading;

namespace GameLauncher.Views {

    public partial class UserPage : BasePage {
        private Screen _viewModel;
        bool loggingOut = false;

        public UserPage () {
            InitializeComponent();

            _viewModel = new UserViewModel();

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(Settings.Default.AccessToken);
            username_label.Content = jwtSecurityToken.Subject;
        }

        private void Logout_Click (object sender, RoutedEventArgs e) {
            if (!loggingOut) {
                loggingOut = true;
                (_viewModel as UserViewModel).Logout();

                Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(async () => {
                    await AnimateOut();
                    (Application.Current.MainWindow as MainWindow).frame.NavigationService.Navigate(new LoginPage());
                    loggingOut = false;
                }));
            }
        }
    }
}
