using System;

namespace bayoen.Utility
{
    public class Gdi32
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}
