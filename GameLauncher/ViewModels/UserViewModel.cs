using Caliburn.Micro;
using DatabaseREST.Models;
using GameLauncher.Properties;
using GameLauncher.Views;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.ViewModels {
    class UserViewModel : Screen {

        private Players player;

        public Players Player {
            get {
                return player;
            }

            set {
                player = value;
                NotifyOfPropertyChange(() => Player);
            }
        }

        public void Logout () {
            Settings.Default.AccessToken = "";
            Settings.Default.RefreshToken = "";
            Settings.Default.Username = "";
            Settings.Default.AutoLogin = false;
            Settings.Default.RememberUsername = false;
            Settings.Default.Save();
        }

        public async Task ExtractPlayerInfo () {
            RestClient client = new RestClient("http://212.10.51.254:30830/api");
            RestRequest request = new RestRequest("/players", Method.GET);

            request.AddHeader("token", Settings.Default.AccessToken);
            request.AddParameter("id", new JwtSecurityToken(Settings.Default.AccessToken).Subject);
            request.AddParameter("full", true);

            var cancellationTokenSource = new CancellationTokenSource();

            try {
                var response = await client.ExecuteAsync(request, cancellationTokenSource.Token);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    var player = JsonConvert.DeserializeObject<Players>(response.Content);

                    Player = player;
                    //DONE
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                    if (await Utility.RefreshToken()) {
                        await ExtractPlayerInfo();
                    }
                    else {
                        ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage("Session expired");
                    }
                }
                else {
                    ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(response.StatusDescription);
                }
            }

            // Exception
            catch (Exception e) {
                ((Application.Current.MainWindow as MainWindow).ViewModel as MainViewModel).DisplayErrorMessage(e.Message);
            }
        }
    }
}
