using Caliburn.Micro;
using GameLauncher.Properties;
using HelperTools;
using Models;
using PatchClientLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
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

        private Task<InstallationDataModel> downloadTask;

        private InstallationDataModel DownloadingModel;

        public bool IsDownloading { get; private set; }

        public static StringCollection GamePaths {
            get {
                StringCollection collection = Settings.Default.GamePaths;

                if (collection == null)
                    collection = new StringCollection();

                else {
                    StringCollection newCollection = new StringCollection();
                    foreach (string path in collection)
                        if (Directory.Exists(path))
                            newCollection.Add(path);

                    collection = newCollection;
                }

                Settings.Default.GamePaths = collection;
                Settings.Default.Save();

                return collection;
            }
        }

        public MainViewModel () {
            //Settings.Default.GamePaths = null;
            //Settings.Default.Save();
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

            private set {
                availableInstalls = value;
                NotifyOfPropertyChange(() => availableInstalls);
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

        public void AddPath (string path) {
            StringCollection paths = GamePaths;
            paths.Add(path);
            Settings.Default.GamePaths = paths;
            Settings.Default.Save();
        }


        /// <summary>
        /// Return the available installs, both locally and remotely
        /// </summary>
        /// <returns></returns>
        private BindableCollection<InstallationDataModel> GetAvailableInstalls (StringCollection paths) {
            return new BindableCollection<InstallationDataModel>(PatchClient.CompleteCheck(paths.Cast<string>().ToArray()));
        }

        public void DownloadSelectedVersion () {
            if (!IsDownloading) {
                IsDownloading = true;
                //Subscribe file downloaded event
                PatchClient.DownloadDone += PatchClient_DownloadDone;
                DownloadingModel = SelectedInstall;
                downloadTask = new Task<InstallationDataModel>(() => PatchClient.DownloadMissingFiles(DownloadingModel));
                downloadTask.Start();
            }
        }

        private void PatchClient_DownloadDone () {
            DownloadingModel = downloadTask.Result;
            if (AvailableInstalls != null) {
                int toChange = -1;
                for (int i = 0; i < availableInstalls.Count; i++) {
                    if (availableInstalls [ i ].VersionName == DownloadingModel.VersionName) {
                        toChange = i;
                        break;
                    }
                }
                if (toChange != -1)
                    AvailableInstalls [ toChange ] = DownloadingModel;
                
            }

            IsDownloading = false;
        }
    }
}
