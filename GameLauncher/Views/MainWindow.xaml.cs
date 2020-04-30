using Caliburn.Micro;
using GameLauncher.Properties;
using GameLauncher.ViewModels;
using Models;
using PatchClientLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }

        private void OnSelectedInstallChanged (InstallationDataModel installationDataModel) {
            Dispatcher.Invoke(() => {
                Delete_Button.Visibility = installationDataModel?.Status == InstallationStatus.Verified || installationDataModel?.Status == InstallationStatus.UpdateRequired ? Visibility.Visible : Visibility.Hidden;
                progress_bar.Visibility = installationDataModel?.Status == InstallationStatus.IsInstalling ? Visibility.Visible : Visibility.Hidden;
                if (progress_bar.Visibility == Visibility.Hidden) progress_bar.NewValueGiven?.Invoke(this, new ProgressBarValueChangedEventArgs((float)progress_bar.Value, 0.0f));
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
                            (ViewModel as MainViewModel).DownloadSelectedVersion();
                        }
                    }

                    break;
                case InstallationStatus.UpdateRequired:
                    (ViewModel as MainViewModel).DownloadSelectedVersion();
                    break;
                default:
                    break;
            }

        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            if (!(ViewModel as MainViewModel).IsDeleting)
                Task.Run(() => (ViewModel as MainViewModel).Delete());
        }
    }
}
