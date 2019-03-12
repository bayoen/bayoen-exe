using System.Windows.Controls;

namespace bayoen.Windows.Layouts
{
    public partial class ColumnGridSplitter : GridSplitter
    {
        public ColumnGridSplitter()
        {
            InitializeComponent();

            this.Width = Config.SplitterThickness;
        }
    }
}
