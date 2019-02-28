using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using mtc = MahApps.Metro.Controls;

namespace bayoen.Windows
{
    /// <summary>
    /// bayoen.Windows.MainWindow
    /// </summary>
    public partial class MainWindow : mtc::MetroWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Dashboard_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }       
}
