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
    class RegisterViewModel : Screen {

        public async Task<bool> Register (string userID, string email, string passwordRaw, string firstName = "Default", string lastName = "Default") {

            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("accounts/", Method.POST);

            //hash password
            string password = Utility.HashedString(passwordRaw);

            //Create object
            Accounts account = new Accounts() {
                AccountId = userID,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = password
            };

            request.AddJsonBody(account);
            var cancellationTokenSource = new CancellationTokenSource();

            try {
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                
                // Created
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    return true;

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



        public static bool CheckPassUniformity (string password, string confirmPass) {
            if (password == confirmPass)
                return true;
            else
                return false;
        }
    }
}
