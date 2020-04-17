using Caliburn.Micro;
using GameLauncher.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.ViewModels {
    public enum TemporaryInstallType { Installed, NotInstalled, UpdateRequired }

    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class TemporaryInstallModel {
        public string VersionName { get; set; }
        public string Path { get; set; }
        public TemporaryInstallType InstallType { get; set; }
    }

    public class MainViewModel : Screen {

        public static StringCollection GamePaths {
            get {
                return Settings.Default.GamePaths;
            }
        }


        private TemporaryInstallModel selectedInstall;
        public TemporaryInstallModel SelectedInstall {
            get { return selectedInstall; }
            set {
                selectedInstall = value;
                NotifyOfPropertyChange(() => SelectedInstall);
            }
        }

        private BindableCollection<TemporaryInstallModel> availableInstalls = null;
        public BindableCollection<TemporaryInstallModel> AvailableInstalls {
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
        private BindableCollection<TemporaryInstallModel> GetAvailableInstalls (StringCollection paths) {
            return new BindableCollection<TemporaryInstallModel>() {
                new TemporaryInstallModel { VersionName = "Version one", InstallType = TemporaryInstallType.Installed},
                new TemporaryInstallModel { VersionName = "Version two", InstallType = TemporaryInstallType.NotInstalled},
                new TemporaryInstallModel { VersionName = "Version three", InstallType = TemporaryInstallType.UpdateRequired},
                new TemporaryInstallModel { VersionName = "Version four", InstallType = TemporaryInstallType.Installed}
            };
        }
    }
}
