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
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => this._selectedTabIndex;
            set
            {
                if (this._selectedTabIndex == value) return;

                for (int tabIndex = 0; tabIndex < this.ContentGrid.Children.Count; tabIndex++)
                {
                    this.ContentGrid.Children[tabIndex].Visibility = (tabIndex == value) ? Visibility.Visible : Visibility.Collapsed;
                }

                this._selectedTabIndex = value;
            }
        }

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

            this.SetVersion($"{Config.ProjectAssemply.Name} v{Config.ProjectAssemply.Version.ToString()}");
            this.Status("Ready");
            this._selectedTabIndex = -1;
            this.SelectedTabIndex = 0;

            this.HomeTabGrid.RecentMatchViewer.CheckGrid();
            this.StatsTabGrid.MatchViewer.CheckGrid();
        }

        public void Status(string s)
        {
            this.Status(s, false);
        }
        public void Status(string s, bool indeterminate)
        {
            this.Status(s, 1000, 1000, indeterminate); // ###.#% resolution
        }
        public void Status(string s, double percent)
        {
            this.Status(s, 1000*percent, 1000, false);
        }
        public void Status(string s, double value, double max)
        {
            this.Status(s, value, max, false);
        }
        public void Status(string s, double value, double max, bool indeterminate)
        {
            this.StatusBorder.StatusTextBlock.Text = s;

            if (indeterminate)
            {
                this.StatusBorder.StatusBar.Visibility = Visibility.Collapsed;
                this.StatusBorder.IndeterminateBar.Visibility = Visibility.Visible;
            }
            else
            {
                this.StatusBorder.StatusBar.Visibility = Visibility.Visible;
                this.StatusBorder.IndeterminateBar.Visibility = Visibility.Collapsed;

                this.StatusBorder.StatusBar.Value = value;
                this.StatusBorder.StatusBar.Maximum = max;
            }
        }

        public void SetVersion(string s)
        {
            this.StatusBorder.VersionTextBlock.Text = s;
        }
    }
}
