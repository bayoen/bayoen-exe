using System;
using System.Collections.Generic;
using System.IO;
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

using bayoen.Data;

namespace bayoen.Windows.Contents
{
    public partial class MatchViewerGrid : Grid
    {
        public MatchViewerGrid() : this(false) { }

        public MatchViewerGrid(bool isRecentOnly)
        {
            InitializeComponent();

            this.Matches = new List<MatchRecord>();

            this.MatchIndex = 0;
            this.IsRecentOnly = isRecentOnly;
        }

        public List<MatchRecord> Matches { get; private set; }

        public int MatchIndex { get; private set; }

        public bool IsRecentOnly { get; set; }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => this._isEmpty;
            set
            {
                if (this._isEmpty == value) return;

                if (value)
                {
                    this.PrevPageButton.IsEnabled = false;
                    this.NextPageButton.IsEnabled = false;
                    this.PageTextBlock.Text = $"(empty) ";
                }
                else
                {
                    this.PrevPageButton.IsEnabled = true;
                    this.NextPageButton.IsEnabled = true;
                }

                this._isEmpty = value;
            }
        }

        public void CheckMatchDataGrid()
        {
            this.DataGrid.ItemsSource = null;

            if (Directory.Exists(Config.StatFolderName))
            {
                if (!this.PrevPageButton.IsEnabled) this.PrevPageButton.IsEnabled = true;
                if (!this.NextPageButton.IsEnabled) this.NextPageButton.IsEnabled = true;

                List<string> files = Directory.GetFiles(Config.StatFolderName, "match_*.json").ToList();
                files.Reverse();

                if (files.Count == 0)
                {
                    this.IsEmpty = true;
                    return;
                }

                int pageCount = (int)Math.Ceiling((double)files.Count / (double)Config.MatchMax);

                int index;
                if (this.MatchIndex >= pageCount) index = 0;
                else if (this.MatchIndex < 0) index = pageCount - 1;
                else index = this.MatchIndex;

                this.PageTextBlock.Text = $"{index + 1}/{pageCount}";

                int entryBegin = index * Config.MatchMax;
                int entryEnd = Math.Min(files.Count, (index + 1) * Config.MatchMax) - 1;
                files = files.GetRange(entryBegin, entryEnd - entryBegin + 1);

                this.IsEmpty = false;
                this.DataGrid.ItemsSource = this.Matches = new List<MatchRecord>(files.ConvertAll(x => MatchRecord.Load(x)));
            }
            else
            {
                this.IsEmpty = true;
            }
        }

        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.MatchIndex--;
            this.CheckMatchDataGrid();
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.MatchIndex++;
            this.CheckMatchDataGrid();
        }

        private void DataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            //e.DetailsElement
        }
    }
}
