﻿using System;
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

namespace GameLauncher {
    /// <summary>
    /// Interaction logic for LoggingInPage.xaml
    /// </summary>
    public partial class LoggingInPage : Page {
        public LoggingInPage (System.Security.SecureString password, string username, bool? rememberUsername, bool v) {
            InitializeComponent();
        }
    }
}
