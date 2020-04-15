using GameLauncher.Properties;
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
using winForms = System.Windows.Forms;

namespace GameLauncher {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        Button playInstallButton;
        public Frame ContentFrame;

        public MainWindow () {
            InitializeComponent();

            playInstallButton = PlayInstall_Button;
            ContentFrame = frame;
            frame.NavigationService.Navigate(new RegisterPage());

            UpdatePlayInstallButtonText();            
        }

        private void PlayInstallButton_Clicked (object sender, RoutedEventArgs e) {
            if (!Logic.PathSelected) {

                using (var dialog = new winForms.FolderBrowserDialog()) {
                    winForms.DialogResult folderLocation = dialog.ShowDialog();

                    // Get the selected file name and display in a TextBox
                    if (folderLocation == System.Windows.Forms.DialogResult.OK) {
                        string filename = dialog.SelectedPath;
                        Settings.Default.GamePath = filename;
                        Settings.Default.Save();
                        UpdatePlayInstallButtonText();
                    }
                }
            }
        }

        private void UpdatePlayInstallButtonText () {
            if (!Logic.GameInstalled)
                playInstallButton.Content = "Install";
            else
                playInstallButton.Content = "Play";
        }
    }
}
