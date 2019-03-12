using System.Linq;
using System.Windows;
using System.Windows.Input;
using bayoen.Utility.Win32;
using bayoen.Windows.Layouts;

namespace bayoen.Windows
{
    public partial class FloatingWindow : OverlayWindow
    {
        public FloatingWindow()
        {
            InitializeComponent();

            this.PuzzleLeagueResultPanel.CheckScore();
        }

        public void Check()
        {
            this.UpdateLocation(this.CheckPPTRect(), this.CheckPPTStatus());

            if (Core.PPTStatus.MainState == Data.Enums.MainStates.PuzzleLeague)
            {
                this.PuzzleLeagueResultPanel.Visibility = Visibility.Visible;
            }
            else
            {
                this.PuzzleLeagueResultPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void OverlayWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Core.PPTMemory.Check())
            {
                User32.SetForegroundWindow(Core.PPTMemory.Process.MainWindowHandle);
            }
        }
    }
}
