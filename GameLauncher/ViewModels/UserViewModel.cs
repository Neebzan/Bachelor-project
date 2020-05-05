using Caliburn.Micro;
using GameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.ViewModels {
    class UserViewModel : Screen {
        public void Logout () {
            Settings.Default.AccessToken = "";
            Settings.Default.RefreshToken = "";
            Settings.Default.Username = "";
            Settings.Default.AutoLogin = false;
            Settings.Default.RememberUsername = false;
            Settings.Default.Save();
        }
    }
}
