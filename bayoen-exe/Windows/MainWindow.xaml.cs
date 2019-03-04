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

            this.Background = new ImageBrush(bayoen.Properties.Resources.img_wallpaper.ToImageSource())
            {
                //Viewbox = new Rect(0,0, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight),
                //ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top,
                Opacity = 0.08,
                Stretch = Stretch.UniformToFill,
            };

            //this.MatchNavigator.CheckMatchDataGrid();
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

    public static partial class ExtendedMethods
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this System.Drawing.Icon icon)
        {
            return HBitmapToImageSource(icon.ToBitmap().GetHbitmap());
        }

        public static ImageSource ToImageSource(this System.Drawing.Bitmap bitmap)
        {
            return HBitmapToImageSource(bitmap.GetHbitmap());
        }

        private static ImageSource HBitmapToImageSource(IntPtr hBitmap)
        {
            ImageSource src = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }

            return src;
        }
    }
}
