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

namespace bayoen.Windows.Layouts
{
    public partial class MatchViewerGrid : Grid
    {
        public MatchViewerGrid()
        {
            InitializeComponent();

            this.Matches = new List<MatchRecord>();

            this.MatchIndex = 0;
        }

        public List<MatchRecord> Matches { get; private set; }

        public int MatchIndex { get; private set; }

        private bool _isRecentOnly;
        public bool IsRecentOnly
        {
            get => this._isRecentOnly;
            set
            {
                this.PagePanel.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                this._isRecentOnly = value;
                this.MatchIndex = -1;
            }
        }

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => this._isEmpty;
            set
            {
                if (this._isEmpty == value) return;

                if (!this.IsRecentOnly)
                {
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
                }

                this._isEmpty = value;
            }
        }

        public void CheckGrid()
        {
            this.DataGrid.ItemsSource = null;

            if (Directory.Exists(Config.StatFolderName))
            {
                List<string> files = Directory.GetFiles(Config.StatFolderName, "match_*.json").ToList();
                if (files.Count == 0)
                {
                    this.IsEmpty = true;
                    return;
                }
                files.Reverse();

                int index;
                if (this.IsRecentOnly)
                {
                    index = 0;
                }
                else
                {
                    int pageCount = (int)Math.Ceiling((double)files.Count / (double)Config.MatchMax);
                    if (this.MatchIndex >= pageCount) index = this.MatchIndex = 0;
                    else if (this.MatchIndex < 0) index = this.MatchIndex = pageCount - 1;
                    else index = this.MatchIndex;
                    this.PageTextBlock.Text = $"{index + 1}/{pageCount}";
                }

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
            this.CheckGrid();
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            this.MatchIndex++;
            this.CheckGrid();
        }

        private void DataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            //e.DetailsElement.
        }
    }
}
