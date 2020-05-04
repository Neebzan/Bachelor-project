using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using DatabaseREST.Models;
using System.Windows;
using GameLauncher.Views;
using System.Threading;
using System.Security;
using System.Runtime.InteropServices;

namespace GameLauncher.ViewModels {
    class LoggingInViewModel : Screen {

        private string _username;
        private string _password;

        public LoggingInViewModel (string username, SecureString passwordRaw) {
            try {
                _username = username;
                _password = Utility.ConvertToUnsecureString(passwordRaw);
            }
            catch (Exception e) {
                //Throw for now, should be handleded
                throw;
            }
        }

        public async Task<bool> Login () {

            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("accounts/login", Method.POST);

            //hash password
            string password = Utility.HashedString(_password);

            //Create object
            Accounts account = new Accounts() {
                AccountId = _username,
                PasswordHash = password
            };

            request.AddJsonBody(account);
            var cancellationTokenSource = new CancellationTokenSource();

            try {
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);

                // Logged in
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    return true;
                }

                // Already exists
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage("Account already exists");

                // Something else
                else
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(response.StatusDescription);
            }

            // Exception
            catch (Exception e) {
                ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(e.Message);
            }

            return false;
        }
    }
}
