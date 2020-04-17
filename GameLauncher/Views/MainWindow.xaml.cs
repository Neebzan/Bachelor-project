using Caliburn.Micro;
using GameLauncher.Properties;
using GameLauncher.ViewModels;
using Models;
using PatchClientLib;
using System;
using System.Collections.Generic;
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
            //Version_ComboBox.Loaded += Version_ComboBox_Loaded;

            ContentFrame = frame;
            frame.NavigationService.Navigate(new LoginPage());

            PlayInstall_Button.Content = "";
            Version_ComboBox.SelectionChanged += Version_ComboBox_SelectionChanged;
        }

        //private void Version_ComboBox_Loaded (object sender, RoutedEventArgs e) {
        //    ControlTemplate ct = Version_ComboBox.Template;
        //    Popup pop = ct.FindName("PART_Popup", this.Version_ComboBox) as Popup;
        //    pop.Placement = PlacementMode.Top;
        //}

        private void Version_ComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            switch ((viewModel as MainViewModel).SelectedInstall.Status) {
                case InstallationStatus.Verified:
                    UpdateButton("Play", Brushes.Green);
                    break;
                case InstallationStatus.NotInstalled:
                    UpdateButton("Install", Brushes.Red);
                    break;
                case InstallationStatus.UpdateRequired:
                    UpdateButton("Update", Brushes.Orange);
                    break;
                default:
                    break;
            }
        }

        private void UpdateButton (string text, SolidColorBrush color) {
            PlayInstall_Button.Background = color;
            PlayInstall_Button.Content = text;
        }

        private void PlayInstallButton_Clicked (object sender, RoutedEventArgs e) {
            switch ((viewModel as MainViewModel).SelectedInstall.Status) {
                case InstallationStatus.NotInstalled:
                    using (var dialog = new winForms.FolderBrowserDialog()) {
                        winForms.DialogResult folderLocation = dialog.ShowDialog();

                        if (folderLocation == System.Windows.Forms.DialogResult.OK) {
                            string path = dialog.SelectedPath;
                            path += "/" + (viewModel as MainViewModel).SelectedInstall.VersionName;
                            (viewModel as MainViewModel).SelectedInstall.InstallPath = path;
                            (viewModel as MainViewModel).AddPath(path);
                            (viewModel as MainViewModel).DownloadVersion();
                        }
                    }

                    break;
                default:
                    break;
            }

        }
    }
}
