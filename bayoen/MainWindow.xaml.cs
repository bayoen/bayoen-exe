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

using mtc = MahApps.Metro.Controls;
using wf = System.Windows.Forms;

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

        public bool isNewMatch;

        public List<int> currentStar;
        public List<int> oldStar;
        #endregion

        #region Sub-Variables
        public wf::NotifyIcon notify;

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


            this.SetNotifyIcon();
        }

        private void SetNotifyIcon()
        {
            this.notify = new wf::NotifyIcon()
            {
                Visible = true,
                Icon = bayoen.Properties.Resources.CarbuncleBadge,
                Text = "bayoen~",
            };

            this.notify.Click += (sender, e) =>
            {

            };
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

            this.pptMemory.CheckProcess();
            int scoreAddress = this.pptMemory.ReadInt32(new IntPtr(0x14057F048)) + 0x38;
            int playerAddress = this.pptMemory.ReadInt32(new IntPtr(this.pptMemory.ReadInt32(new IntPtr(0x140473760)) + 0x20)) + 0xD8;
            bool isNoMatch = (scoreAddress == 0x38);

            //


            this.Tik.Stop();
            this.Status(string.Format("Working [Process per {0} ms]", this.Tik.ElapsedMilliseconds) + (this.tickFlag ? "." : ".."));
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
        #endregion

        #region Control Functions
        private void StatOnButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsStatOn = false;
        }

        private void StatOffButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsStatOn = true;
        }
        #endregion

        #region Contants
        public const int playerMax = 4;
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
