using GameLauncher.Properties;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : BasePage {
        string savedUsername;

        public LoginPage () {
            InitializeComponent();
            spinner_imageawesome.Visibility = Visibility.Hidden;

            this.Loaded += CheckAutoLogin;

            savedUsername = Settings.Default.Username;
            remember_username_tick.IsChecked = Settings.Default.RememberUsername;
            automatic_login_tick.IsChecked = Settings.Default.AutoLogin;
            if (remember_username_tick.IsChecked == true && !string.IsNullOrEmpty(savedUsername)) {
                usernameEmail_textblock.Text = savedUsername;
            }
        }

        private void login_button_Click (object sender, RoutedEventArgs e) {
            spinner_imageawesome.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(password_passwordBox.Password) && !string.IsNullOrEmpty(usernameEmail_textblock.Text)) {
                SecureString password = password_passwordBox.SecurePassword;
                string username = usernameEmail_textblock.Text;
                bool? rememberUsername = remember_username_tick.IsChecked;

                Dispatcher.Invoke(DispatcherPriority.Background,
                    new Action(async () => {
                        await AnimateOut();
                        (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new LoggingInPage(password, username, rememberUsername, false));
                    }));
            }
            else {
                //(Application.Current.MainWindow as MainWindow).DisplayError("Login request failed", "You must enter both a password and a username in order to login");
                spinner_imageawesome.Visibility = Visibility.Hidden;
            }
        }

        private async void register_button_Click (object sender, RoutedEventArgs e) {
            await AnimateOut();
            (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new RegisterPage());
        }

        private async void CheckAutoLogin (object sender, RoutedEventArgs e) {
            if (automatic_login_tick.IsChecked == true) {
                if (!string.IsNullOrEmpty(Settings.Default.Username) && !string.IsNullOrEmpty(Logic.ConvertToUnsecureString(Settings.Default.Password))) {
                    await AnimateOut();
                    SecureString password = password_passwordBox.SecurePassword;
                    string username = usernameEmail_textblock.Text;
                    bool? rememberUsername = remember_username_tick.IsChecked;
                    (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new LoggingInPage(password, username, rememberUsername, true));
                }
            }
        }

        private void remember_username_tick_Unchecked (object sender, RoutedEventArgs e) {
            if (!string.IsNullOrEmpty(savedUsername)) {
                savedUsername = string.Empty;
            }
            Settings.Default.Username = string.Empty;
            Settings.Default.RememberUsername = false;
            Settings.Default.Save();
            automatic_login_tick.IsChecked = false;
        }

        private void remember_username_tick_Checked (object sender, RoutedEventArgs e) {
            Settings.Default.RememberUsername = true;
            Settings.Default.Save();
        }

        private void automatic_login_tick_Unchecked (object sender, RoutedEventArgs e) {
            Settings.Default.AutoLogin = false;
            remember_username_tick.IsEnabled = true;
            Settings.Default.Save();
        }

        private void automatic_login_tick_Checked (object sender, RoutedEventArgs e) {
            Settings.Default.AutoLogin = true;
            remember_username_tick.IsChecked = true;
            remember_username_tick.IsEnabled = false;
            Settings.Default.Save();
        }
    }
}
