using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;
using mtc = MahApps.Metro.Controls;
using wf = System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace bayoen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : mtc::MetroWindow
    {
        #region Core-Variables
        public VAMemory pptMemory;
        public PPTimer Timer;
        public Stopwatch Tik;
        private bool tickFlag;

        public List<int> currentStar;
        public List<int> oldStar;
        public List<Player> players;

        public bool isNewMatch;
        public bool isDoneMatch;
        public Match currentMatch;

        public List<int> countingStars;
        public List<int> countingGames;

        public jl::JArray matchArray;
        #endregion

        #region Sub-Variables
        public wf::NotifyIcon notify;
        public mtc::MetroWindow MetroPopup;
        public TextBlock PopupP1StarTextBlock;
        public TextBlock PopupP2StarTextBlock;
        public TextBlock PopupP1GameTextBlock;
        public TextBlock PopupP2GameTextBlock;

        public Process[] PPTProcesses;

        private bool _isStatOn;
        public bool IsStatOn
        {
            get => this._isStatOn;
            set
            {
                if (value)
                {
                    this.StatOnButton.Visibility = Visibility.Visible;
                    this.StatOffButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.StatOnButton.Visibility = Visibility.Collapsed;
                    this.StatOffButton.Visibility = Visibility.Visible;
                }

                this._isStatOn = value;
            }
        }

        private bool _isCountingStarOn;
        public bool IsCountingStarOn
        {
            get => this._isCountingStarOn;
            set
            {
                if (value)
                {
                    this.CountingStarOnButton.Visibility = Visibility.Visible;
                    this.CountingStarOffButton.Visibility = Visibility.Collapsed;
                    //this.MinWidth += 410;
                    this.Width = Math.Min(this.Width + this.CDS0.Width.Value + this.Col1.Width.Value, System.Windows.SystemParameters.WorkArea.Width);
                    this.Left -= Math.Max(0, this.Left + this.Width - System.Windows.SystemParameters.WorkArea.Width);
                    this.CDS0.MinWidth = this.CDS0.MaxWidth = this.CDS0.Width.Value;
                    this.Col1.MinWidth = this.Col1.MaxWidth = this.Col1.Width.Value;
                }
                else
                {
                    this.CountingStarOnButton.Visibility = Visibility.Collapsed;
                    this.CountingStarOffButton.Visibility = Visibility.Visible;
                    //this.MinWidth = Math.Max(this.MinWidth-410, this.Col0.MinWidth);
                    this.Width = Math.Max(this.Width - this.Col1.Width.Value - this.CDS0.Width.Value, this.MinWidth);
                    this.CDS0.MinWidth = this.CDS0.MaxWidth = 0;
                    this.Col1.MinWidth = this.Col1.MaxWidth = 0;
                }


                this._isCountingStarOn = value;
            }
        }
        #endregion

        public MainWindow()
        {
            this.InitializeComponent();
            this.InitializeLayouts();
            this.InitializeTimer();
            this.InitializeVariables();

            this.IsStatOn = false;
            this.Status("Ready!");
        }

        private void InitializeLayouts()
        {
            //this.Col1.

            this.IsCountingStarOn = false;

            this.SetNotifyIcon();
            this.SetMetroPopup();

            // Centering
            Rect workingRect = System.Windows.SystemParameters.WorkArea;
            this.Left = (workingRect.Width - this.Width) / 2 + workingRect.Left;
            this.Top = (workingRect.Height - this.Height) / 2 + workingRect.Top;
        }

        private void AddMatchItem(Match match)
        {
            // Match -> text, image, etc..
            Grid MatchItemGrid = new Grid()
            {
                Background = Brushes.MediumVioletRed,
                MinHeight = 55,
                MinWidth = 200,
            };

            CheckBox MatchCheckBox = new CheckBox()
            {

            };
            MatchItemGrid.Children.Add(MatchCheckBox);

            TextBlock MatchScoreTextBlock = new TextBlock()
            {
                FontSize = 16,
                FontWeight = FontWeights.ExtraBold,
                Text = string.Format("[{0}]", string.Join(":", match.Stars)),
                Margin = new Thickness(3),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            MatchItemGrid.Children.Add(MatchScoreTextBlock);

            TextBlock MatchPlayerTextBlock = new TextBlock()
            {
                FontSize = 12,
                FontWeight = FontWeights.ExtraBold,
                Text = string.Join(", ", match.Players.ConvertAll(x => x.Name)),
                Margin = new Thickness(3, 30, 3, 3),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            MatchItemGrid.Children.Add(MatchPlayerTextBlock);

            TextBlock MatchDateTextBlock = new TextBlock()
            {
                FontSize = 12,
                Text = string.Format("{0}{1}", (match.IsBroken.Value ? "Broken" : ""), (match.MatchStart != null ? match.MatchStart.Value.ToLocalTime().ToString() : "")),
                Margin = new Thickness(3),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            MatchItemGrid.Children.Add(MatchDateTextBlock);




            this.MatchItemPanel.Children.Insert(0, MatchItemGrid);
            //return true;
        }

        private bool SetCurrentMatch(Match match)
        {
            this.CurrentStar1.Text = match.Stars[0].ToString();
            this.CurrentStar2.Text = match.Stars[1].ToString();
            this.CurrentStar3.Text = match.Stars[2].ToString();
            this.CurrentStar4.Text = match.Stars[3].ToString();

            return true;
        }

        private bool SetCounting()
        {
            this.CountingStar1.Text = this.countingStars[0].ToString();
            this.CountingStar2.Text = this.countingStars[1].ToString();
            this.CountingStar3.Text = this.countingStars[2].ToString();
            this.CountingStar4.Text = this.countingStars[3].ToString();

            this.CountingGame1.Text = this.countingGames[0].ToString();
            this.CountingGame2.Text = this.countingGames[1].ToString();
            this.CountingGame3.Text = this.countingGames[2].ToString();
            this.CountingGame4.Text = this.countingGames[3].ToString();

            // SemiBoard2
            this.PopupP1StarTextBlock.Text = this.countingStars[0].ToString();
            this.PopupP2StarTextBlock.Text = this.countingStars[1].ToString();

            this.PopupP1GameTextBlock.Text = this.countingGames[0].ToString();
            this.PopupP2GameTextBlock.Text = this.countingGames[1].ToString();


            return true;
        }

        private void SetNotifyIcon()
        {
            this.notify = new wf::NotifyIcon()
            {
                Visible = true,
                Icon = bayoen.Properties.Resources.CarbuncleBadge,
                Text = "bayoen~",
            };

            this.notify.MouseDoubleClick += (sender, e) =>
            {
                this.ShowMainWindow();
            };

            this.notify.ContextMenu = new wf::ContextMenu();

            wf::MenuItem OpenMenu = new wf::MenuItem()
            {
                Text = "Open",
            };
            OpenMenu.Click += (sender, e) =>
            {
                this.ShowMainWindow();
            };
            this.notify.ContextMenu.MenuItems.Add(OpenMenu);
            wf::MenuItem ExitMenu = new wf::MenuItem()
            {
                Text = "Exit",
            };
            ExitMenu.Click += (sender, e) =>
            {
                this.ExitMainWindow();
            };
            this.notify.ContextMenu.MenuItems.Add(ExitMenu);
        }

        private void SetMetroPopup()
        {
            this.MetroPopup = new mtc::MetroWindow()
            {
                Title = "Counting Stars: Head TWO Head",
                TitleCharacterCasing = CharacterCasing.Normal,
                Height = 150,
                Width = 450,
                Background = Brushes.Magenta,
                ResizeMode = ResizeMode.NoResize,                
            };

            this.MetroPopup.Closing += (sender, e) =>
            {
                e.Cancel = true;
                this.MetroPopup.Hide();
            };

            Grid SemiBoard2Grid = new Grid()
            {

            };
            this.MetroPopup.Content = SemiBoard2Grid;

            Image panelImage = new Image()
            {
                Height = 68,
                Width = 406,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            SemiBoard2Grid.Children.Add(panelImage);
            using (MemoryStream streamToken = new MemoryStream())
            {
                bayoen.Properties.Resources.SemiBoard_2P.Save(streamToken, System.Drawing.Imaging.ImageFormat.Png);
                streamToken.Position = 0;

                BitmapImage bitmapToken = new BitmapImage();
                bitmapToken.BeginInit();
                bitmapToken.CacheOption = BitmapCacheOption.OnLoad;
                bitmapToken.StreamSource = streamToken;
                bitmapToken.EndInit();

                panelImage.Source = bitmapToken;
                bitmapToken.Freeze();
            }

            StackPanel SemiBoard2TextPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            SemiBoard2Grid.Children.Add(SemiBoard2TextPanel);

            this.PopupP1GameTextBlock = NewTextBlock();
            this.PopupP1GameTextBlock.Margin = new Thickness(25, 23, 10, 24);
            SemiBoard2TextPanel.Children.Add(this.PopupP1GameTextBlock);
            this.PopupP1StarTextBlock = NewTextBlock();
            this.PopupP1StarTextBlock.Margin = new Thickness(24, 23, 6, 24);
            SemiBoard2TextPanel.Children.Add(this.PopupP1StarTextBlock);
            this.PopupP2StarTextBlock = NewTextBlock();
            this.PopupP2StarTextBlock.Margin = new Thickness(7, 23, 24, 24);
            SemiBoard2TextPanel.Children.Add(this.PopupP2StarTextBlock);
            this.PopupP2GameTextBlock = NewTextBlock();
            this.PopupP2GameTextBlock.Margin = new Thickness(13, 23, 26, 24);
            SemiBoard2TextPanel.Children.Add(this.PopupP2GameTextBlock);

            TextBlock NewTextBlock()
            {
                return new TextBlock()
                {
                    Width = 40,
                    Text = "-",
                    FontSize = 28,                    
                    FontWeight = FontWeights.ExtraBold,
                    TextAlignment = TextAlignment.Center,
                };
            }
        }

        private void InitializeTimer()
        {
            this.Timer = new PPTimer(10, 1, 4);
            this.Timer.Tick += (e, sender) =>
            {
                this.Timer.Ring++;

                if (!this.IsStatOn)
                {
                    Status("Paused");
                    return;
                }

                if (this.Timer.StepAFlag)
                {
                    //// Check Offline
                    if (CheckPPTState() != PPTStates.Running)
                    {
                        this.Status(string.Format("Offline") + (this.tickFlag ? "." : ".."));
                        this.tickFlag = !tickFlag;
                        return;
                    }
                }

                if (this.Timer.StepBFlag)
                {
                    this.Tick();
                    this.tickFlag = !tickFlag;
                }
            };
            this.Timer.Start();

            this.tickFlag = false;
            this.Tik = new Stopwatch();
        }

        private void Tick()
        {
            this.Tik.Reset();
            this.Tik.Start();

            // Check Puyo Puyo Tetris process
            if (!this.pptMemory.CheckProcess())
            {
                this.Tik.Stop();
                this.Status(string.Format("Fail to catch Puyo Puyo Tetris.. [{0} ms]", this.Tik.ElapsedMilliseconds) + (this.tickFlag ? "." : ".."));
                return;
            }
            
            // Set initial address
            int scoreAddress = this.pptMemory.ReadInt32(new IntPtr(0x14057F048)) + 0x38;
            int playerAddress = this.pptMemory.ReadInt32(new IntPtr(this.pptMemory.ReadInt32(new IntPtr(0x140473760)) + 0x20)) + 0xD8;
            bool isNoMatch = (scoreAddress == 0x38);


            // Check database
            if (this.matchArray == null)
            {
                if (File.Exists(matchJSONName))
                {
                    try
                    {
                        this.matchArray = jl::JArray.Parse(File.ReadAllText(matchJSONName, Encoding.Unicode));
                    }
                    catch
                    {
                        this.matchArray = new jl::JArray();
                        File.WriteAllText(matchJSONName, this.matchArray.ToString(), Encoding.Unicode);
                    }
                }
                else
                {
                    this.matchArray = new jl::JArray();
                    File.WriteAllText(matchJSONName, this.matchArray.ToString(), Encoding.Unicode);
                }

                for (int matchIndex = 0; matchIndex < this.matchArray.Count; matchIndex++)
                {
                    this.AddMatchItem(Match.FromJSON(jl::JObject.FromObject(this.matchArray[matchIndex])));
                }
            }

            if (isNoMatch)
            {
                this.Tik.Stop();
                this.Status(string.Format("Wating for game [{0} ms]", this.Tik.ElapsedMilliseconds) + (this.tickFlag ? "." : ".."));
                return;
            }

            // for all players (P1-P4)
            for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
            {
                this.currentStar[playerIndex] = this.pptMemory.ReadInt32(new IntPtr(scoreAddress) + playerIndex * 0x4); // Read current stars
            }


            this.isDoneMatch = false;
            // Check new match
            if (this.isNewMatch)
            {
                // Do off flag
                this.isNewMatch = false;

                if (this.currentStar.Sum() == 0)
                {
                    this.currentMatch = new Match()
                    {
                        MatchStart = DateTime.UtcNow,
                        FirstToStar = this.pptMemory.ReadInt32(new IntPtr(scoreAddress) + 0x10),
                    };                    
                }
                else
                {
                    this.oldStar = new List<int>(this.currentStar);

                    //// Recover broken match
                    // new match
                    this.currentMatch = new Match()
                    {
                        IsBroken = true,
                        Stars = new List<int>(this.oldStar), // replace star of winner

                        MatchStart = DateTime.UtcNow,
                        FirstToStar = this.pptMemory.ReadInt32(new IntPtr(scoreAddress) + 0x10),
                    };

                    
                    int overwhelmingIndex = this.oldStar.IndexOf(this.oldStar.Sum());
                    if (overwhelmingIndex > -1) // if someone overwhelmed?
                    {
                        for (int winningIndex = 0; winningIndex < this.oldStar[overwhelmingIndex]; winningIndex++)
                        {
                            for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
                            {
                                this.currentMatch.Players[playerIndex].Stars.Add((playerIndex == overwhelmingIndex) ? (MatchResults.Win) : (MatchResults.Lose)); // Did the player overwhelm?
                            }
                        }
                    }
                    else
                    {
                        // add pseudo result
                        for (int starIndex = 0; starIndex < playerMax; starIndex++)
                        {
                            for (int winningIndex = 0; winningIndex < this.oldStar[starIndex]; winningIndex++)
                            {
                                for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
                                {
                                    this.currentMatch.Players[playerIndex].Stars.Add((playerIndex == starIndex) ? (MatchResults.PseudoWin) : (MatchResults.PseudoLose)); // Did the player win anyway?
                                }
                            }
                        }
                    }

                    if (this.oldStar.IndexOf(this.currentMatch.FirstToStar.Value) > -1) // if the player get max star
                    {
                        this.isDoneMatch = true;
                    }
                }

                //// Get players static infos.
                for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
                {
                    this.currentMatch.Players[playerIndex].Name = this.pptMemory.ReadStringUnicode(new IntPtr(0x140598BD4) + playerIndex * 0x68, 0x24).Replace("\u0000", "");
                }
            }
            else
            {
                if (this.currentStar.Sum() == 0)
                {
                    //
                    this.currentMatch.FirstToStar = this.pptMemory.ReadInt32(new IntPtr(scoreAddress) + 0x10);

                    //// Get players static infos.
                    for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
                    {
                        this.currentMatch.Players[playerIndex].Name = this.pptMemory.ReadStringUnicode(new IntPtr(0x140598BD4) + playerIndex * 0x68, 0x24).Replace("\u0000", "");
                    }
                }
            }


            // Calculate gradient and increase scores
            List<int> gradients = this.currentStar.Zip(this.oldStar, (a, b) => a - b).ToList();
            if (gradients.IndexOf(1) > -1) // if someone get a star
            {
                for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
                {
                    this.currentMatch.Players[playerIndex].Stars.Add((gradients[playerIndex] == 1) ? (MatchResults.Win) : (MatchResults.Lose)); // Did the player win?

                    if (gradients[playerIndex] == 1) this.currentMatch.Stars[playerIndex]++; // increase star of winner

                    if (this.currentMatch.FirstToStar == this.currentStar[playerIndex]) // if the player get max star
                    {
                        if (!this.isDoneMatch) this.isDoneMatch = true;
                        if (this.currentMatch.IsBroken == null) this.currentMatch.IsBroken = false;
                    }
                }

                List<int> overallStars = new List<int>(playerMax) { 0, 0, 0, 0 };
                List<int> overallGames = new List<int>(playerMax) { 0, 0, 0, 0 };
                foreach (jl::JObject tokenJObject in this.matchArray)
                {
                    Match tokenMatch = Match.FromJSON(tokenJObject);
                    overallStars = overallStars.Zip(tokenMatch.Stars, (a, b) => a + b).ToList();
                    overallGames = overallGames.Zip(tokenMatch.Stars.ConvertAll(x => x / tokenMatch.FirstToStar.Value), (a, b) => a + b).ToList();
                }

                this.countingStars = overallStars.Zip(this.currentMatch.Stars, (a, b) => a + b).ToList();
                this.countingGames = overallGames.Zip(this.currentMatch.Stars.ConvertAll(x => x / this.currentMatch.FirstToStar.Value), (a, b) => a + b).ToList();

            }
            else if (gradients.FindIndex(x => x < 0) > -1) // if reset
            {
                this.isNewMatch = true;
            }
            this.oldStar = new List<int>(this.currentStar);

            // 
            //this.TextOut.Text = string.Format("this.currentMatch.ToJSON():\n{0}", this.currentMatch.ToJSON().ToString());
            this.SetCurrentMatch(this.currentMatch);
            this.SetCounting();

            // Finish match
            if (this.isDoneMatch)
            {
                this.currentMatch.MatchEnd = DateTime.UtcNow;
                this.matchArray.Add(this.currentMatch.ToJSON());
                File.WriteAllText(matchJSONName, this.matchArray.ToString(), Encoding.Unicode);                
                this.AddMatchItem(Match.FromJSON(jl::JObject.FromObject(this.currentMatch.ToJSON())));

                this.isDoneMatch = false;

            }
            else
            {
                jl::JArray tokenArray = new jl::JArray(this.matchArray)
                {
                    this.currentMatch.ToJSON(),
                };
                File.WriteAllText(matchJSONName, tokenArray.ToString(), Encoding.Unicode);
            }

            this.Tik.Stop();
            this.Status(string.Format("Working [{0} ms]", this.Tik.ElapsedMilliseconds) + (this.tickFlag ? "." : ".."));


            
        }

        private PPTStates CheckPPTState()
        {
            this.PPTProcesses = Process.GetProcessesByName(pptName);
            if (this.PPTProcesses.Length == 0)
            {
                return PPTStates.Closed;
            }
            else if (this.PPTProcesses.Length > 1)
            {
                return PPTStates.Multiple;
            }

            return PPTStates.Running;
        }



        private void InitializeVariables()
        {
            this.pptMemory = new VAMemory(pptName);

            this.matchArray = null;

            this.players = new List<Player>();
            for (int tokenIndex = 0; tokenIndex < playerMax; tokenIndex++)
            {
                Player tokenPlayer = new Player()
                {
                    Position = tokenIndex + 1,
                };

                this.players.Add(tokenPlayer);
            }

            this.isNewMatch = true;
            this.oldStar = new List<int>(playerMax) { 0, 0, 0, 0 };
            this.currentStar = new List<int>(playerMax) { 0, 0, 0, 0 };

            this.countingStars = new List<int>(playerMax) { 0, 0, 0, 0 };
            this.countingGames = new List<int>(playerMax) { 0, 0, 0, 0 };
        }        

        #region Sub-Functions
        private void Status(string s)
        {
            Status(s, this.StatusBar.Maximum, this.StatusBar.Maximum);
        }

        private void Status(string s, double value, double max)
        {
            this.StatusTextBlock.Text = s;
            this.StatusBar.Value = value;
            this.StatusBar.Maximum = max;
        }

        private void Warning(string s)
        {
            Status(s);
            HandSound();
        }

        private void BeepSound()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        private void HandSound()
        {
            System.Media.SystemSounds.Hand.Play();
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        void ShowMainWindow()
        {
            this.Show();
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            this.Activate();
        }

        void ExitMainWindow()
        {
            Environment.Exit(0);
        }
        #endregion

        #region Control Functions
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(matchJSONName))
            {
                try
                {
                    File.Delete(matchJSONName);
                }
                catch
                {
                    this.Warning(string.Format("{0} can not be deleted", matchJSONName));
                    return;
                }

            }

            if (this.matchArray != null)
            {
                this.matchArray.Clear();
                this.matchArray = null;
            }

            this.MatchItemPanel.Children.Clear();
            GC.Collect(0);

            this.countingStars = new List<int>(playerMax) { 0, 0, 0, 0 };
            this.countingGames = new List<int>(playerMax) { 0, 0, 0, 0 };
            if (this.currentMatch != null) this.currentMatch.Clear();
        }


        ////
        private void CountingStarOnButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsCountingStarOn = false;
        }

        private void CountingStarOffButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsCountingStarOn = true;
        }


        ////
        private void StatOnButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsStatOn = false;
        }

        private void StatOffButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsStatOn = true;
        }


        private void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            this.MetroPopup.Show();
            if (this.MetroPopup.WindowState == WindowState.Minimized)
            {
                this.MetroPopup.WindowState = WindowState.Normal;
            }
            this.MetroPopup.Activate();
        }
        #endregion

        #region Contants
        public const int playerMax = 4;
        public const string matchJSONName = "matches.json";
        public const string pptName = "puyopuyotetris";

        public enum PlayTypes : int
        {
            PuyoPuyo = 0,
            Tetris = 1,
            Swap = 2,
            Fusion = 3,
        }

        public enum MatchTypes : int
        {
            PuzzleLeague = 0,
            FreePlay = 1,
            Singleplay = 2,
            Multiplay = 3,
        }

        public enum MatchModes : int
        {
            Versus = 0,
            Swap = 1,
            BigBang = 2,
            Party = 3,
            Fusion = 4,
        }

        public enum MatchResults
        {
            Win,
            Lose,
            PseudoWin,
            PseudoLose,
        }

        public enum Leagues : int
        {
            Student = 0,
            Beginner = 1,
            Rookie = 2,
            Amateur = 3,
            Ace = 4,
            Wizard = 5,
            Professional = 6,
            Elite = 7,
            Virtuoso = 8,
            Star = 9,
            Superstar = 10,
            Legend = 11,
            Golden = 12,
            Platinum = 13,
            Grand_Master = 14,
        }

        private enum PPTStates
        {
            Closed,
            //Busy,
            Running,
            //Hidden,
            Multiple,
            Robby,
            Game,
        }

        /// <summary>
        /// Get current color of theme highlight.
        /// </summary>
        public static Color HighlightColor
        {
            get { return (Color)MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current).Item2.Resources["HighlightColor"]; }
        }

        /// <summary>
        /// Get current color of theme accent.
        /// </summary>
        public static Color AccentColor
        {
            get { return (Color)MahApps.Metro.ThemeManager.DetectAppStyle(Application.Current).Item2.Resources["AccentColor"]; }
        }

        /// <summary>
        /// Get 1-dimmed current color of theme accent.
        /// </summary>
        public static Color AccentDimColor1
        {
            get
            {
                double downRate = 1.5;
                return new Color() { A = AccentColor.A, B = (byte)(AccentColor.B / downRate), G = (byte)(AccentColor.G / downRate), R = (byte)(AccentColor.R / downRate) };
            }
        }

        /// <summary>
        /// Get 2-dimmed current color of theme accent.
        /// </summary>
        public static Color AccentDimColor2
        {
            get
            {
                double downRate = Math.Pow(1.5, 2);
                return new Color() { A = AccentColor.A, B = (byte)(AccentColor.B / downRate), G = (byte)(AccentColor.G / downRate), R = (byte)(AccentColor.R / downRate) };
            }
        }

        /// <summary>
        /// Get 3-dimmed current color of theme accent.
        /// </summary>
        public static Color AccentDimColor3
        {
            get
            {
                double downRate = Math.Pow(1.5, 3);
                return new Color() { A = AccentColor.A, B = (byte)(AccentColor.B / downRate), G = (byte)(AccentColor.G / downRate), R = (byte)(AccentColor.R / downRate) };
            }
        }

        /// <summary>
        /// Get current brush of theme highlight.
        /// </summary>
        public static Brush HighlightBrush
        {
            get { return new SolidColorBrush(HighlightColor); }
        }

        /// <summary>
        /// Get current brush of theme accent.
        /// </summary>
        public static Brush AccentBrush
        {
            get { return new SolidColorBrush(AccentColor); }
        }

        /// <summary>
        /// Get 1-dimmed current brush of theme accent.
        /// </summary>
        public static Brush AccentDimBrush1
        {
            get { return new SolidColorBrush(AccentDimColor1); }
        }

        /// <summary>
        /// Get 2-dimmed current brush of theme accent.
        /// </summary>
        public static Brush AccentDimBrush2
        {
            get { return new SolidColorBrush(AccentDimColor2); }
        }

        /// <summary>
        /// Get 3-dimmed current brush of theme accent.
        /// </summary>
        public static Brush AccentDimBrush3
        {
            get { return new SolidColorBrush(AccentDimColor3); }
        }


        #endregion
    }
}
