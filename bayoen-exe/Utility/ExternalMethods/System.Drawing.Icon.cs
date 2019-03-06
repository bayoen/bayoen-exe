using System.Drawing;
using System.Windows.Media;

namespace bayoen.Utility.ExternalMethods
{
    public static partial class ExtendedMethods
    {
        public static ImageSource ToImageSource(this Icon icon) => ExtendedMethods.HBitmapToImageSource(icon.ToBitmap().GetHbitmap());
    }
}
