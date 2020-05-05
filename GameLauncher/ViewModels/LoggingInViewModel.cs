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
using RestSharp.Extensions;
using Newtonsoft.Json;
using GameLauncher.Properties;
using RestSharp.Serialization.Json;
using RestSharp.Serializers.NewtonsoftJson;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GameLauncher.ViewModels {
    class LoggingInViewModel : Screen {
        private const string URL = "http://212.10.51.254:30830";

        public async Task<bool> LoginTokenAsync () {
            RestClient client = new RestClient("http://212.10.51.254:30830/api");

            RestRequest request = new RestRequest("token", Method.POST);
            request.AddHeader("token", Settings.Default.RefreshToken);

            var cancellationTokenSource = new CancellationTokenSource();

            try {
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var tokens = JsonConvert.DeserializeObject<TokenModel>(response.Content);

                    Settings.Default.AccessToken = tokens.AccessToken;
                    Settings.Default.Save();

                    return true;
                }

                // Something else
                else {
                    Settings.Default.AccessToken = Settings.Default.RefreshToken = "";
                    Settings.Default.Save();
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(response.StatusDescription);
                }
            }

            // Exception
            catch (Exception e) {
                ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(e.Message);
            }

            return false;
        }


        public async Task<bool> LoginAsync (string username, SecureString rawPassword) {
            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("accounts/login", Method.POST);

            //Convert to unsecure
            string unsecurePassword = Utility.ConvertToUnsecureString(rawPassword);
            //hash password
            string hashedPassword = Utility.HashedString(unsecurePassword);

            //Create object
            Accounts account = new Accounts() {
                AccountId = username,
                PasswordHash = hashedPassword
            };

            request.AddJsonBody(account);

            var cancellationTokenSource = new CancellationTokenSource();

            try {
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var tokens = JsonConvert.DeserializeObject<TokenModel>(response.Content);

                    Settings.Default.AccessToken = tokens.AccessToken;
                    Settings.Default.RefreshToken = tokens.RefreshToken;
                    Settings.Default.Save();

                    return true;
                }

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
