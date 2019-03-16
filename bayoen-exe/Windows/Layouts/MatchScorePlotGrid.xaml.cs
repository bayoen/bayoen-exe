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

using oxy = OxyPlot;
using bayoen.Data;
using bayoen.Utility.ExternalMethods;

namespace bayoen.Windows.Layouts
{
    public partial class MatchScorePlotGrid : Grid
    {
        private MatchRecord MatchRecord;
        public oxy::PlotModel GameScorePlotModel { get; private set; }        

        public MatchScorePlotGrid()
        {
            InitializeComponent();

            this.GameScorePlotModel = new oxy::PlotModel()
            {
                Background = oxy::OxyColors.Transparent,

                LegendBorder = oxy::OxyColors.Black,
                LegendBorderThickness = 1,
                LegendPosition = oxy::LegendPosition.LeftTop,                
            };
            this.GameScorePlotView.Model = this.GameScorePlotModel;
        }

        public void Clear()
        {
            this.GameListView.ItemsSource = null;
            this.GameScorePlotView.Model = null;
        }

        public bool Set(MatchRecord matchRecord)
        {
            this.MatchRecord = matchRecord;

            if (this.MatchRecord.Games == null) return false;
            else if (this.MatchRecord.Games.Count == 0) return false;

            this.GameListView.ItemsSource = null;
            List<int> list = new List<int>(Enumerable.Range(1, this.MatchRecord.Games.Count));
            this.GameListView.ItemsSource = list;
            this.GameListView.SelectedIndex = 0;

            return PlotGameScore(this.MatchRecord, 0);
        }

        private bool PlotGameScore(MatchRecord matchRecord, int gameIndex)
        {
            GameRecord gameRecord = matchRecord.Games[gameIndex];
            if (gameRecord == null) return false;

            this.GameScorePlotView.Model = null;
            this.GameScorePlotModel.Series.Clear();

            for (int playerIndex = 0; playerIndex < matchRecord.Players.Count; playerIndex++)
            {
                PlayerInfo playerInfo = matchRecord.Players[playerIndex];
                PlayerRecord playerRecord = gameRecord.PlayerRecords[playerIndex];
                List<oxy::DataPoint> points = new List<oxy::DataPoint>(gameRecord.Ticks.Zip(playerRecord.Scores, (t, s) =>
                {
                    return new oxy::DataPoint(t / 60, s);
                }));

                bool myFlag = playerInfo.ID32 == Core.PPTMemory.MyID32;

                string colorHex = Config.ScorePlotColorHexes[playerIndex];

                oxy::Series.LineSeries series = new oxy::Series.LineSeries()
                {
                    ItemsSource = points,
                    Title = $"{playerInfo.Name} ({((myFlag) ? "Me, " : "")}{playerRecord.Scores.Last()})",
                    Color = oxy::OxyColor.Parse(colorHex),
                    LineStyle = (myFlag) ? oxy::LineStyle.Dash : oxy::LineStyle.Solid,
                };

                this.GameScorePlotModel.Title = $"Game {gameIndex+1}, {gameRecord.GameEnd.ToLocalTime().ToSimpleString()}";
                this.GameScorePlotModel.Series.Add(series);
            }

            this.GameScorePlotView.Model = this.GameScorePlotModel;
            return true;
        }

        private void GameListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = (sender as ListView).SelectedIndex;
            if (selectedIndex > -1)
            {
                this.PlotGameScore(this.MatchRecord, selectedIndex);
            }            
        }
    }
}
