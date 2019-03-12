using System.Windows;
using System.Windows.Media;

using bayoen.Utility.Win32;

using mtc = MahApps.Metro.Controls;

namespace bayoen.Windows.Layouts
{
    public class OverlayWindow : mtc::MetroWindow
    {
        public bool IsCapturable { get; set; }

        private bool _isClosed;
        public bool IsClosed
        {
            get => this._isClosed;
            set
            {
                if (this._isClosed == value) return;

                if (value)
                {
                    this.Hide();
                }
                else
                {
                    this.Show();
                }

                this._isClosed = value;
            }
        }

        public OverlayWindow()
        {
            this.BorderThickness = new System.Windows.Thickness(0);
            this.ShowTitleBar = false;
            this.TitleBarHeight = 0;

            this._isClosed = true;
        }

        public WindowStatus CheckPPTStatus()
        {
            int currentWindowStyle = User32.GetWindowLong(Core.PPTMemory.Process.MainWindowHandle, (int)GWL.GWL_STYLE);

            if ((currentWindowStyle & (uint)WS.WS_MINIMIZE) == (uint)WS.WS_MINIMIZE)
            {
                return WindowStatus.Minimized;
            }
            else if ((currentWindowStyle & (uint)WS.WS_MAXIMIZE) == (uint)WS.WS_MAXIMIZE)
            {
                return WindowStatus.Maximized;
            }
            else if (User32.GetForegroundWindow() == Core.PPTMemory.Process.MainWindowHandle)
            {
                return WindowStatus.Focused;
            }
            else
            {
                return WindowStatus.Lost;
            }
        }

        public RECT CheckPPTRect()
        {
            RECT pptRect = new RECT();
            User32.GetWindowRect(Core.PPTMemory.Process.MainWindowHandle, ref pptRect);
            return pptRect;
        }

        private const int offsetTop = -31;
        private const int offsetLeft = -8;
        private const int offsetHeight = 39;
        private const int offsetWidth = 16;

        private static double unitHeight { get => SystemParameters.PrimaryScreenHeight - (double)offsetHeight; }
        private static double unitWidth { get => SystemParameters.PrimaryScreenWidth - (double)offsetWidth; }

        public bool UpdateLocation(RECT pptRect, WindowStatus pptStatus)
        {
            int validHeight = pptRect.Height - offsetHeight;
            int validWidth = pptRect.Width - offsetWidth;

            double scaleY = (double)validHeight / unitHeight;
            double scaleX = (double)validWidth / unitWidth;

            if (pptRect.Width == SystemParameters.PrimaryScreenWidth && pptRect.Height == SystemParameters.PrimaryScreenHeight)
            {
                (this.Content as FrameworkElement).LayoutTransform = new ScaleTransform(1, 1);
                this.Top = 0;
                this.Left = 0;
                this.Height = SystemParameters.PrimaryScreenHeight;
                this.Width = SystemParameters.PrimaryScreenWidth;

                return true;
            }
            else if (validHeight <= 0 || validWidth <= 0 || pptStatus == WindowStatus.Minimized || pptStatus == WindowStatus.Closed)
            {
                this.Visibility = Visibility.Hidden;
                return false;
            }
            else
            {
                if (!this.IsCapturable)
                {
                    this.Topmost = false;
                    this.Topmost = (pptStatus == WindowStatus.Focused);
                }

                int validTop = pptRect.Top - offsetTop;
                int validLeft = pptRect.Left - offsetLeft;

                if (this.Visibility != Visibility.Visible) this.Visibility = Visibility.Visible;

                if (this.Top != validTop) this.Top = validTop;
                if (this.Left != validLeft) this.Left = validLeft;
                if (this.Height != validHeight && this.Width != validWidth)
                {
                    (this.Content as FrameworkElement).LayoutTransform = new ScaleTransform(scaleX, scaleY);
                    this.Height = validHeight;
                    this.Width = validWidth;
                }

                return true;
            }
        }
    }
}
