using Caliburn.Micro;
using GameLauncher.Properties;
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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class RegisterPage : BasePage {
        Screen _viewModel;

        public RegisterPage () {
            InitializeComponent();
            spinner_imageawesome.Visibility = Visibility.Hidden;
            _viewModel = new RegisterViewModel();
            this.DataContext = _viewModel;
        }

        private async void register_button_Click (object sender, RoutedEventArgs e) {
            spinner_imageawesome.Visibility = Visibility.Visible;

            //If required fields are not empty
            if (!string.IsNullOrEmpty(password_passwordBox.Password) && !string.IsNullOrEmpty(username_textblock.Text) && !string.IsNullOrEmpty(passwordConfirm_passwordBox.Password)) {
                if (Utility.ConvertToUnsecureString(password_passwordBox.SecurePassword).Contains(" ") || username_textblock.Text.Contains(" ") || email_textblock.Text.Contains(" ")) {
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage("Required fields cannot contain spaces");
                    spinner_imageawesome.Visibility = Visibility.Hidden;
                }
                else {
                    SecureString password = password_passwordBox.SecurePassword;
                    SecureString confirmPass = passwordConfirm_passwordBox.SecurePassword;
                    string username = username_textblock.Text;
                    string email = email_textblock.Text;
                    string firstName = "";
                    string lastName = "";
                    await CreateUserRequest(password, confirmPass, username, email, firstName, lastName);
                }
            }
            else {
                //(Application.Current.MainWindow as MainWindow).DisplayError("Create user request failed", "You must fill out all fields");
                ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage("You must fill out all required fields");
                spinner_imageawesome.Visibility = Visibility.Hidden;
            }
        }

        private async Task CreateUserRequest (SecureString password, SecureString confirmPass, string userID, string email, string firstName, string lastName) {
            //Convert to unsecure
            string unsecurePassword = Utility.ConvertToUnsecureString(password);
            string unsecureConfirmPass = Utility.ConvertToUnsecureString(confirmPass);

            //Check for uniformity
            if (RegisterViewModel.CheckPassUniformity(unsecurePassword, unsecureConfirmPass)) {
                if (await (_viewModel as RegisterViewModel).Register(userID, email, unsecurePassword)) {
                    Settings.Default.Username = userID;
                    Settings.Default.Save();

                    Dispatcher.Invoke(DispatcherPriority.Background,
                    new Action(async () => {
                        await AnimateOut();
                        (Application.Current.MainWindow as MainWindow).frame.NavigationService.Navigate(new LoginPage());
                    }));
                }
            }
            else {
                spinner_imageawesome.Visibility = Visibility.Hidden;
                ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage("The passwords did not match");
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
