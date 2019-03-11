using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using bayoen.Utility.Win32;

namespace bayoen.Utility.ExternalMethods
{
    public static partial class ExtendedMethods
    {
        private static ImageSource HBitmapToImageSource(IntPtr hBitmap)
        {
            ImageSource src = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!Gdi32.DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return src;
        }
    }
}
