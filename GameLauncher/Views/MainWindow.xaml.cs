using Caliburn.Micro;
using GameLauncher.ViewModels;
using Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using winForms = System.Windows.Forms;

namespace GameLauncher.Views {


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public Frame ContentFrame;
        public Screen ViewModel;


        public MainWindow () {
            InitializeComponent();

            ViewModel = new MainViewModel();
            this.DataContext = ViewModel;


            ContentFrame = frame;
            frame.NavigationService.Navigate(new LoginPage());

            progress_bar.Visibility = Visibility.Hidden;
            Delete_Button.Visibility = Visibility.Hidden;

            (ViewModel as MainViewModel).DownloadProgressUpdated += OnDownloadProgressUpdated;
            (ViewModel as MainViewModel).SelectedInstallUpdated += OnSelectedInstallChanged;
            (ViewModel as MainViewModel).DownloadCompleted += OnDownloadCompleted;

        }

        private void OnSelectedInstallChanged (InstallationDataModel installationDataModel) {
            Dispatcher.Invoke(() => {
                Delete_Button.Visibility = installationDataModel?.Status == InstallationStatus.Verified || installationDataModel?.Status == InstallationStatus.UpdateRequired ? Visibility.Visible : Visibility.Hidden;
            });
        }

        private void OnDownloadCompleted () {
            Dispatcher.Invoke(() => {
                progress_bar.Visibility = Visibility.Hidden;
                PlayInstall_Button.IsEnabled = true;
            });
        }
        private void OnDownloadProgressUpdated (float obj) {
            Dispatcher.Invoke(() => {
                progress_bar.NewValueGiven?.Invoke(this, new ProgressBarValueChangedEventArgs((float)progress_bar.Value, obj));
            });
        }


        private void PlayInstallButton_Clicked (object sender, RoutedEventArgs e) {
            switch ((ViewModel as MainViewModel).SelectedInstall.Status) {
                case InstallationStatus.NotInstalled:
                    using (var dialog = new winForms.FolderBrowserDialog() { RootFolder = Environment.SpecialFolder.Desktop, Description = "Please select where you want to install the game." }) {
                        winForms.DialogResult folderLocation = dialog.ShowDialog();
                        if (folderLocation == System.Windows.Forms.DialogResult.OK) {
                            string path = dialog.SelectedPath;
                            path += "\\" + (ViewModel as MainViewModel).SelectedInstall.VersionName;
                            (ViewModel as MainViewModel).SelectedInstall.InstallPath = path;
                            (ViewModel as MainViewModel).AddPath(path);
                            InitiateDownload();
                        }
                    }

                    break;
                case InstallationStatus.UpdateRequired:
                    InitiateDownload();
                    break;

                case InstallationStatus.IsInstalling:
                    break;

                default:
                    break;
            }
        }

        private void InitiateDownload () {
            (ViewModel as MainViewModel).DownloadSelectedVersion();
            Dispatcher.Invoke(() => {
                progress_bar.Visibility = Visibility.Visible;
                progress_bar.NewValueGiven?.Invoke(this, new ProgressBarValueChangedEventArgs((float)progress_bar.Value, 0.0f));
                PlayInstall_Button.IsEnabled = false;
            });
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            if (!(ViewModel as MainViewModel).IsDeleting)
                Task.Run(() => (ViewModel as MainViewModel).Delete());
        }
    }
}
