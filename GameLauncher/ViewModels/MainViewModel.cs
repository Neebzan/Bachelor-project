using Caliburn.Micro;
using GameLauncher.Properties;
using Models;
using PatchClientLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.ViewModels {
    public enum TemporaryInstallType { Installed, NotInstalled, UpdateRequired }

    //[SettingsSerializeAs(SettingsSerializeAs.Xml)]
    //public class TemporaryInstallModel {
    //    public string VersionName { get; set; }
    //    public string Path { get; set; }
    //    public TemporaryInstallType InstallType { get; set; }
    //}

    public class MainViewModel : Screen {

        public bool IsDownloading { get; private set; }

        public static StringCollection GamePaths {
            get {
                StringCollection collection = Settings.Default.GamePaths;
                if (collection == null) {
                    collection = new StringCollection();
                    Settings.Default.GamePaths = collection;
                }
                Settings.Default.Save();
                return Settings.Default.GamePaths;
            }
        }

        public MainViewModel () {
            Settings.Default.GamePaths = null;
            Settings.Default.Save();
        }

        private InstallationDataModel selectedInstall;
        public InstallationDataModel SelectedInstall {
            get { return selectedInstall; }
            set {
                selectedInstall = value;
                NotifyOfPropertyChange(() => SelectedInstall);
            }
        }

        private BindableCollection<InstallationDataModel> availableInstalls = null;
        public BindableCollection<InstallationDataModel> AvailableInstalls {
            get {
                if (availableInstalls == null)
                    availableInstalls = GetAvailableInstalls(GamePaths);

                return availableInstalls;
            }
        }


        int temporaryProgress = 213;
        int temporaryTotal = 1231;

        public string DownloadProgress {
            get {
                return "Progress: " + ((Convert.ToSingle(temporaryProgress) / Convert.ToSingle(temporaryTotal)) * 100.0f).ToString("0.00") + "%";
            }
        }

        public string DownloadFileName {
            get {
                return "Current file: Some file name";
            }
        }

        public bool AddPath (string path) {
            StringCollection paths = GamePaths;
            if (!paths.Contains(path)) {
                paths.Add(path);
                return true;

            }
            return false;
        }


        /// <summary>
        /// Return the available installs, both locally and remotely
        /// </summary>
        /// <returns></returns>
        private BindableCollection<InstallationDataModel> GetAvailableInstalls (StringCollection paths) {
            return new BindableCollection<InstallationDataModel>(PatchClient.CompleteCheck(paths.Cast<string>().ToArray()));
        }

        public void DownloadVersion () {
            if (!IsDownloading) {
                IsDownloading = true;

                //Subscribe file downloaded event
                PatchClient.DownloadDone += PatchClient_DownloadDone;
                Task.Run(() => PatchClient.DownloadMissingFiles(SelectedInstall));
            }
        }

        private void PatchClient_DownloadDone () {
            
            IsDownloading = false;
        }
    }
}
