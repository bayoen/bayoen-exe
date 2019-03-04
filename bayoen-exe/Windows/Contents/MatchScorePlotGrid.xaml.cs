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

using op = OxyPlot;

namespace bayoen.Windows.Contents
{
    public partial class MatchScorePlotGrid : Grid
    {
        public op::PlotModel GameScorePlotModel { get; private set; }

        public MatchScorePlotGrid()
        {
            InitializeComponent();

            this.GameScorePlotModel = new op::PlotModel();
            this.GameScorePlotModel.Series.Add(new op::Series.FunctionSeries(Math.Sin, 0, Math.PI * 8, 200, "sin(x)"));

            this.GameScorePlotView.Model = this.GameScorePlotModel;
        }
    }
}
