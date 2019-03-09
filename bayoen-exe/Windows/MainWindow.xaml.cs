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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using bayoen.Utility.ExternalMethods;
using mtc = MahApps.Metro.Controls;


namespace bayoen.Windows
{
    public partial class MainWindow : mtc::MetroWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();

            this.Background = new ImageBrush(bayoen.Properties.Resources.img_wallpaper.ToImageSource())
            {
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Opacity = 0.08,
                Stretch = Stretch.UniformToFill,
            };

            this.HomeTabGrid.RecentNavigator.CheckGrid();
        }

        private void Dashboard_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Dashboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
