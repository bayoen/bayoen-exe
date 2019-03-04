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

using bayoen.Utility;

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

            if (!Gdi32.DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }

            return src;
        }
    }
}
