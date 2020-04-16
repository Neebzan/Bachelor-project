using Caliburn.Micro;
using GameLauncher.Properties;
using PatchClientLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using winForms = System.Windows.Forms;

namespace GameLauncher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Button playInstallButton;
        public Frame ContentFrame;
        public BindableCollection<string> AvailableVersions { get; set; }

        public MainWindow () {
            InitializeComponent();
            Settings.Default.GamePath = "";
            Settings.Default.Save();

            playInstallButton = PlayInstall_Button;
            ContentFrame = frame;
            frame.NavigationService.Navigate(new RegisterPage());
            playInstallButton.IsEnabled = !Logic.PathSelected;

            AvailableVersions = new BindableCollection<string>(PatchClient.serverVersions);
            PatchClient.VersionVerificationDone += PatchClient_VersionVerificationDone;
            PatchClient.VersionsFromServerReceived += PatchClient_VersionsFromServerReceived;
            PatchClient.DownloadDone += PatchClient_DownloadDone;

            //UpdatePlayInstallButtonText();
            PatchClient.RequestAvailableVersions();
            Logic.CheckInstall();
        }

        private void PatchClient_DownloadDone () {
            
        }

        private void PatchClient_VersionsFromServerReceived (VersionsFromServerRecievedEventArgs obj) {
            string version = obj.Versions [ 0 ];
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => {Install_label.Content = version; AvailableVersions = new BindableCollection<string>(PatchClient.serverVersions); }));
        }

        private void PatchClient_VersionVerificationDone () {
            playInstallButton.IsEnabled = true;
        }

        private void PlayInstallButton_Clicked (object sender, RoutedEventArgs e) {
            if (!Logic.PathSelected) {

                using (var dialog = new winForms.FolderBrowserDialog()) {
                    winForms.DialogResult folderLocation = dialog.ShowDialog();

                    // Get the selected file name and display in a TextBox
                    if (folderLocation == System.Windows.Forms.DialogResult.OK) {
                        string filename = dialog.SelectedPath;
                        Logic.NewPath(filename);

                        //UpdatePlayInstallButtonText();
                    }
                }
            }
        }

        //private void UpdatePlayInstallButtonText () {
        //    if (!Logic.GameInstalled)
        //        playInstallButton.Content = "Install";
        //    else
        //        playInstallButton.Content = "Play";
        //}
    }
}
