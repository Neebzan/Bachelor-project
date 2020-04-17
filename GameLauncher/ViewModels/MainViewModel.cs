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
using System.Windows;
using System.Windows.Threading;

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
            PatchClient.GetDownloadProgress += PatchClient_GetDownloadProgress;
            PatchClient.DownloadDone += PatchClient_DownloadDone;
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
                NotifyOfPropertyChange(() => AvailableInstalls);
            }
        }

        private string downloadProgress = "";

        public string DownloadProgress {
            get { return downloadProgress; }
            set { downloadProgress = value; NotifyOfPropertyChange(() => DownloadProgress); }
        }

        private string downloadFile = "";

        public string DownloadFile {
            get { return downloadFile; }
            set { downloadFile = value; NotifyOfPropertyChange(() => DownloadFile); }
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
                Task.Run(() => PatchClient.DownloadMissingFiles(SelectedInstall));
            }
        }

        private void PatchClient_GetDownloadProgress (object sender, DownloadProgressEventArgs e) {
            DownloadProgress = ((Convert.ToSingle(e.DownloadedTotal) / Convert.ToSingle(e.TotalSize)) * 100.0f).ToString("0.00") + "%";
            DownloadFile = e.NextFileName;
        }

        private void PatchClient_DownloadDone (InstallationDataModel installation) {
            if (AvailableInstalls != null) {
                int toChange = -1;

                for (int i = 0; i < availableInstalls.Count; i++) {
                    if (availableInstalls [ i ].VersionName == installation.VersionName)
                        toChange = i; break;
                }

                if (toChange != -1)
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { AvailableInstalls [ toChange ] = installation; SelectedInstall = AvailableInstalls [ toChange ]; }));
            }

            IsDownloading = false;
            DownloadFile = "";
            DownloadProgress = "";
        }
    }
}
