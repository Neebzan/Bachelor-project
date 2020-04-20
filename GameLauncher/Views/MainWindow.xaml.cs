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
        private Screen viewModel;


        public MainWindow () {
            InitializeComponent();

            viewModel = new MainViewModel();
            this.DataContext = viewModel;


            ContentFrame = frame;
            frame.NavigationService.Navigate(new LoginPage());

            progress_bar.Visibility = Visibility.Hidden;

            (viewModel as MainViewModel).DownloadProgressUpdated += OnDownloadProgressUpdated;
            (viewModel as MainViewModel).SelectedInstallUpdated += OnSelectedInstallChanged;
        }

        private void OnSelectedInstallChanged (InstallationDataModel installationDataModel) {
            Dispatcher.Invoke(() => {
                Delete_Button.Visibility = installationDataModel?.Status == InstallationStatus.Verified || installationDataModel?.Status == InstallationStatus.UpdateRequired ? Visibility.Visible : Visibility.Hidden;
                progress_bar.Visibility = installationDataModel?.Status == InstallationStatus.IsInstalling ? Visibility.Visible : Visibility.Hidden;
            });
        }

        private void OnDownloadProgressUpdated (float obj) {
            Dispatcher.Invoke(() => {
                progress_bar.NewValueGiven?.Invoke(this, new ProgressBarValueChangedEventArgs((float)progress_bar.Value, obj));
            });
        }

        private void PlayInstallButton_Clicked (object sender, RoutedEventArgs e) {
            switch ((viewModel as MainViewModel).SelectedInstall.Status) {
                case InstallationStatus.NotInstalled:
                    using (var dialog = new winForms.FolderBrowserDialog() { RootFolder = Environment.SpecialFolder.Desktop, Description = "Please select where you want to install the game." }) {
                        winForms.DialogResult folderLocation = dialog.ShowDialog();
                        if (folderLocation == System.Windows.Forms.DialogResult.OK) {
                            string path = dialog.SelectedPath;
                            path += "\\" + (viewModel as MainViewModel).SelectedInstall.VersionName;
                            (viewModel as MainViewModel).SelectedInstall.InstallPath = path;
                            (viewModel as MainViewModel).AddPath(path);
                            (viewModel as MainViewModel).DownloadSelectedVersion();
                        }
                    }

                    break;
                case InstallationStatus.UpdateRequired:
                    (viewModel as MainViewModel).DownloadSelectedVersion();
                    break;
                default:
                    break;
            }

        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            if (!(viewModel as MainViewModel).IsDeleting)
                Task.Run(() => (viewModel as MainViewModel).Delete());
        }
    }
}
