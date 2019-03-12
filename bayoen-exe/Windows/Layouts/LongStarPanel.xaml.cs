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
using bayoen.Utility.ExternalMethods;

namespace bayoen.Windows.Layouts
{
    public partial class LongStarPanel : Grid
    {
        public LongStarPanel()
        {
            InitializeComponent();

            this.PanelImage.SetBitmap(bayoen.Properties.Resources.pnl_star_500);
            //this.WinImage.SetBitmap(bayoen.Properties.Resources.cpn_win);
            //this.LoseImage.SetBitmap(bayoen.Properties.Resources.cpn_lose);
            //this.RatingImage.SetBitmap(bayoen.Properties.Resources.cpn_rating);
        }

        public void SetScore(int wins, int loses, int rating)
        {
            this.WinTextBlock.Text = $"{wins.ToString()}";
            this.LoseTextBlock.Text = $"{loses.ToString()}";
            this.RatingTextBlock.Text = $"{rating.ToString("+#;-#;0")}";
        }

        public void CheckScore()
        {
            if (Directory.Exists(Config.StatFolderName))
            {
                List<string> files = Directory.GetFiles(Config.StatFolderName, "match_*.json").ToList();
                if (files.Count == 0)
                {
                    this.SetScore(0, 0, 0);
                    return;
                }
                files.Reverse();

                int wins, loses, rating;
                wins = loses = rating = 0;

                foreach (string filePath in files)
                {
                    MatchRecord tokenMatch = MatchRecord.Load(filePath);

                    if (tokenMatch.MatchEnd.ToLocalTime().Date < DateTime.Today) break;

                    if (tokenMatch.RatingGain > 0) wins++;
                    else loses++;

                    rating += tokenMatch.RatingGain;
                }

                this.SetScore(wins, loses, rating);
            }
            else
            {
                this.SetScore(0, 0, 0);
                return;
            }
        }
    }
}
