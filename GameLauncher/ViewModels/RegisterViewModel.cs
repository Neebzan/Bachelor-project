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

namespace GameLauncher.ViewModels {
    class RegisterViewModel : Screen {

        public bool Register (string userID, string passwordRaw, string email, string firstName = "Default", string lastName = "Default") {

            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("accounts/", Method.POST);

            Accounts account = new Accounts() {
                AccountId = userID,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordRaw
            };

            request.AddJsonBody(account);

            try {
                var response = client.Execute(request);

                // Created
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    return true;

                // Already exists
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage("Account already exists");

                // Fuck
                else
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(response.StatusDescription);
            }
            catch (Exception e) {
                // Fuck^Frick
                ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(e.Message);
                throw;
            }

            return false;
        }

    }
}
