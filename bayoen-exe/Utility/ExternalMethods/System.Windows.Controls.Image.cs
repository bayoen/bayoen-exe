﻿using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace bayoen.Utility.ExternalMethods
{
    public static partial class ExtendedMethods
    {
        public static bool SetBitmap(this System.Windows.Controls.Image image, System.Drawing.Bitmap bitmap)
        {
            return image.SetBitmap(bitmap, BitmapScalingMode.Fant);
        }

        public static bool SetBitmap(this System.Windows.Controls.Image image, System.Drawing.Bitmap bitmap, BitmapScalingMode mode)
        {
            try
            {
                using (System.IO.MemoryStream streamToken = new System.IO.MemoryStream())
                {
                    bitmap.Save(streamToken, System.Drawing.Imaging.ImageFormat.Png);
                    streamToken.Position = 0;

                    BitmapImage bitmapImageToken = new BitmapImage();
                    bitmapImageToken.BeginInit();
                    bitmapImageToken.CacheOption = BitmapCacheOption.OnLoad;

                    bitmapImageToken.StreamSource = streamToken;
                    bitmapImageToken.EndInit();

                    image.Source = bitmapImageToken;
                    RenderOptions.SetBitmapScalingMode(image, mode);
                    bitmapImageToken.Freeze();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
