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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace bayoen.Windows.Layouts
{
    public partial class StatsTabGrid : Grid
    {
        public StatsTabGrid()
        {
            InitializeComponent();

            this.MatchViewer.DataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = (sender as DataGrid).SelectedIndex;

            if (selectedIndex > -1)
            {
                this.MatchScorePlot.Set(this.MatchViewer.Matches[selectedIndex]);
            }
            else
            {
                this.MatchScorePlot.Clear();
            }
        }
    }
}
