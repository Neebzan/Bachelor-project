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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SystemTimer = System.Timers.Timer;

namespace GameLauncher.ViewModels {

    public class MainViewModel : Screen {
        #region fields

        public event Action<float> DownloadProgressUpdated;
        public event Action<InstallationDataModel> SelectedInstallUpdated;

        #endregion

        #region Properties
        private string _buttonText = "Fetching..";
        public string ButtonText {
            get { return _buttonText; }
            set {
                _buttonText = value;
                NotifyOfPropertyChange(() => ButtonText);
            }
        }

        private float _downloadProgressPercentage;
        public float DownloadProgressPercentage {
            get { return _downloadProgressPercentage; }
            set {
                _downloadProgressPercentage = value;
                NotifyOfPropertyChange(() => DownloadProgressPercentage);
                DownloadProgressUpdated.Invoke(_downloadProgressPercentage);
            }
        }

        private string _downloadProgress = "";
        public string DownloadProgress {
            get { return _downloadProgress; }
            set {
                _downloadProgress = value;
                NotifyOfPropertyChange(() => DownloadProgress);
            }
        }

        private string _downloadFile = "";
        public string DownloadFile {
            get { return _downloadFile; }
            set { _downloadFile = value; NotifyOfPropertyChange(() => DownloadFile); }
        }

        private string errorMessage = "";
        public string ErrorMessage {
            get { return errorMessage; }
            set {
                errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool IsDeleting { get; private set; }

        private InstallationDataModel _selectedInstall;
        public InstallationDataModel SelectedInstall {
            get { return _selectedInstall; }
            set {
                _selectedInstall = value;
                NotifyOfPropertyChange(() => SelectedInstall);
                Settings.Default.LastSelectedVersion = _selectedInstall?.VersionBranchToString;
                Settings.Default.Save();

                if (IsDownloading) {
                    ButtonText = "installing..";
                }

                else if (!IsDeleting) {
                    switch (SelectedInstall?.Status) {
                        case InstallationStatus.Verified:
                            ButtonText = "Play";
                            break;
                        case InstallationStatus.NotInstalled:
                            ButtonText = "Install";
                            break;
                        case InstallationStatus.UpdateRequired:
                            ButtonText = "Update";
                            break;
                        case InstallationStatus.IsDeleting:
                            ButtonText = "Deleting";
                            break;
                        default:
                            break;
                    }
                    SelectedInstallUpdated.Invoke(SelectedInstall);
                }
            }
        }

        private BindableCollection<InstallationDataModel> _availableInstalls = null;
        public BindableCollection<InstallationDataModel> AvailableInstalls {
            get {
                return _availableInstalls;
            }

            private set {
                _availableInstalls = value;
                NotifyOfPropertyChange(() => AvailableInstalls);
            }
        }

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

        public Action DownloadCompleted { get; internal set; }
        #endregion

        #region Methods
        public MainViewModel () {
            PatchClient.GetDownloadProgress += PatchClient_GetDownloadProgress;
            PatchClient.DownloadDone += PatchClient_DownloadDone;
            Task.Run(() => AvailableInstalls = GetAvailableInstalls(GamePaths));


            if (Settings.Default.AutoLogin && !string.IsNullOrEmpty(Settings.Default.AccessToken)) {
                IssueAutoLogin();
            }

        }

        /// <summary>
        /// Sets an error to display for a duration of time
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="duration"></param>
        public void DisplayErrorMessage (string msg, float duration = 3000) {
            SystemTimer timer = new SystemTimer(duration);
            timer.Start();

            ErrorMessage = msg;
            timer.Elapsed += Timer_Elapsed;
        }
        /// <summary>
        /// Wipes error message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed (object sender, System.Timers.ElapsedEventArgs e) {
            ErrorMessage = "";
        }

        private void IssueAutoLogin () {

        }

        private InstallationDataModel UpdateState (InstallationDataModel model, InstallationStatus status) {
            InstallationDataModel copy = model;
            copy.Status = status;
            return copy;
        }

        public void Delete () {
            IsDeleting = true;

            //From gsharp
            //https://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory

            SelectedInstall.Status = InstallationStatus.IsDeleting;

            try {
                System.IO.DirectoryInfo di = new DirectoryInfo(SelectedInstall.InstallPath);

                foreach (FileInfo file in di.GetFiles()) {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories()) {
                    dir.Delete(true);
                }

                Directory.Delete(SelectedInstall.InstallPath);

                InstallationDataModel newModel = SelectedInstall;

                IsDeleting = false;
                SelectedInstall = UpdateState(SelectedInstall, InstallationStatus.NotInstalled);
            }
            catch (Exception e) {
                //Handle exec
            }
        }

        /// <summary>
        /// Return the available installs, both locally and remotely
        /// </summary>
        /// <returns></returns>
        private BindableCollection<InstallationDataModel> GetAvailableInstalls (StringCollection paths) {
            BindableCollection<InstallationDataModel> available = new BindableCollection<InstallationDataModel>();
            try {
                available = new BindableCollection<InstallationDataModel>(PatchClient.CompleteCheck(paths.Cast<string>().ToArray()));
                if (SelectedInstall == null) {
                    if (!String.IsNullOrEmpty(Settings.Default.LastSelectedVersion)) {
                        foreach (InstallationDataModel installation in available) {
                            if (installation.VersionBranchToString == Settings.Default.LastSelectedVersion) {
                                SelectedInstall = installation;
                                break;
                            }
                        }
                    }
                    if (SelectedInstall == null)
                        SelectedInstall = available [ 0 ];
                }


            }
            catch (Exception e) {
                DisplayErrorMessage(e.Message);
            }
            return available;
        }

        public void DownloadSelectedVersion () {
            if (SelectedInstall.Status != InstallationStatus.IsInstalling) {
                IsDownloading = true;
                SelectedInstall = UpdateState(SelectedInstall, InstallationStatus.IsInstalling);
                DownloadProgressPercentage = 0.0f;
                Task.Run(() => PatchClient.DownloadMissingFiles(SelectedInstall));
            }
        }

        private void PatchClient_GetDownloadProgress (object sender, DownloadProgressEventArgs e) {
            DownloadProgressPercentage = ((Convert.ToSingle(e.DownloadedTotal) / Convert.ToSingle(e.TotalSize)) * 100.0f);
            e.NextFileName = e.NextFileName.Length <= 30 ? e.NextFileName : e.NextFileName.Substring(0, 30);
            DownloadProgress = "Downloading: " + DownloadProgressPercentage.ToString("0.00") + " % (" + e.NextFileName + ")";
        }

        private void PatchClient_DownloadDone (InstallationDataModel installation) {
            IsDownloading = false;
            if (AvailableInstalls != null) {
                int toChange = -1;

                for (int i = 0; i < _availableInstalls.Count; i++) {
                    if (_availableInstalls [ i ].VersionBranch == installation.VersionBranch) {
                        toChange = i;
                        break;
                    }
                }

                if (toChange != -1)
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {
                        AvailableInstalls [ toChange ] = installation;
                        SelectedInstall = AvailableInstalls [ toChange ];
                    }));

            }

            //Also remove bar
            DownloadCompleted?.Invoke();
            DownloadFile = "";
            DownloadProgress = "";
            DownloadProgressPercentage = 100.0f;

        }

        public void AddPath (string path) {
            StringCollection paths = GamePaths;
            paths.Add(path);
            Settings.Default.GamePaths = paths;
            Settings.Default.Save();
        }
        #endregion
    }
}
