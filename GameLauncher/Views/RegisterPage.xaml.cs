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
    public partial class RegisterPage : BasePage {
        public RegisterPage () {
            InitializeComponent();
            spinner_imageawesome.Visibility = Visibility.Hidden;
        }

        private void register_button_Click (object sender, RoutedEventArgs e) {
            spinner_imageawesome.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(password_passwordBox.Password) && !string.IsNullOrEmpty(username_textblock.Text) && !string.IsNullOrEmpty(passwordConfirm_passwordBox.Password)) {
                if (Logic.ConvertToUnsecureString(password_passwordBox.SecurePassword).Contains(" ") ||
                    username_textblock.Text.Contains(" ")) {
                    //(Application.Current.MainWindow as MainWindow).DisplayError("Create user request failed", "Entries must not contain spaces");
                    spinner_imageawesome.Visibility = Visibility.Hidden;
                }
                else {
                    SecureString password = password_passwordBox.SecurePassword;
                    SecureString confirmPass = passwordConfirm_passwordBox.SecurePassword;
                    string username = username_textblock.Text;
                    //Task.Factory.StartNew(() => CreateUserRequest(password, confirmPass, username));
                }
            }
            else {
                //(Application.Current.MainWindow as MainWindow).DisplayError("Create user request failed", "You must fill out all fields");
                spinner_imageawesome.Visibility = Visibility.Hidden;
            }
        }

        private async void CreateUserRequest (SecureString password, SecureString confirmPass, string username) {
            if (Logic.CheckPassUniformity(password, confirmPass)) {
                //if (await Logic.SendRegisterRequest(username, password)) {
                //    Settings.Default.username = username;
                //    Settings.Default.Save();

                //    Dispatcher.Invoke(DispatcherPriority.Background,
                //    new Action(async () => {
                //        await AnimateOut();
                //        (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new LoginPage());
                //    }));
                //}
            }
            else {
                Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(() => {
                    spinner_imageawesome.Visibility = Visibility.Hidden;
                    //(Application.Current.MainWindow as MainWindow).DisplayError("Create user request failed", "Your password entry does not match the confirm password entry");
                }));

            }
            Dispatcher.Invoke(DispatcherPriority.Background,
            new Action(() => {
                spinner_imageawesome.Visibility = Visibility.Hidden;
            }));
        }

        private async void return_button_Click (object sender, RoutedEventArgs e) {
            await AnimateOut();
            (Application.Current.MainWindow as MainWindow).ContentFrame.NavigationService.Navigate(new LoginPage());
        }
    }
}
