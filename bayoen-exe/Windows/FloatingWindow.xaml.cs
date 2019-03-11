using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using bayoen.Utility;
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

        private void OverlayWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Core.PPTMemory.CheckProcess())
            {
                User32.SetForegroundWindow(Core.PPTMemory.GetProcesses().Single().MainWindowHandle);
            }
        }
    }
}
