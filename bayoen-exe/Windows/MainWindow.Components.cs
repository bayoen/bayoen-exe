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
    public partial class MainWindow
    {
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void NavigatorListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedTabIndex = (sender as ListView).SelectedIndex;
        }
    }
}
