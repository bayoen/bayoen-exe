using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// <summary>
    /// MatchNavigator.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MatchNavigatorGrid : Grid
    {
        public List<MatchRecord> Matches { get; private set; }

        public int MatchIndex { get; private set; }

        public MatchNavigatorGrid()
        {
            InitializeComponent();

            this.Matches = new List<MatchRecord>();

            this.MatchIndex = 0;            
        }

        public void CheckMatchDataGrid()
        {
            this.DataGrid.ItemsSource = null;

            if (Directory.Exists(Config.StatFolderName))
            {
                List<string> files = Directory.GetFiles(Config.StatFolderName, "match_*.json").ToList();
                files.Reverse();

                if (files.Count == 0)
                {
                    Empty();
                    return;
                }

                int pageCount = (int)Math.Ceiling((double)files.Count / (double)Config.MatchMax);
                int index = Math.Max(0, Math.Min(pageCount - 1, this.MatchIndex));

                this.PageTextBlock.Text = $"{index + 1}/{pageCount}";
                
                int entryBegin = index * Config.MatchMax;
                int entryEnd = Math.Min(files.Count, (index + 1) * Config.MatchMax) - 1;
                files = files.GetRange(entryBegin, entryEnd - entryBegin + 1);
                
                this.DataGrid.ItemsSource = this.Matches = new List<MatchRecord>(files.ConvertAll(x => MatchRecord.Load(x)));
            }
            else
            {
                Empty();
            }


            void Empty()
            {
                this.PageTextBlock.Text = $"(empty) 1/1";
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
