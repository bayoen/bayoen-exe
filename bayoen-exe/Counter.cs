using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

using mtc = MahApps.Metro.Controls;
using System.Linq;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Windows.Shapes;
using System;
using System.Windows.Input;

namespace bayoen
{
    public partial class MainWindow
    {
        public class Counter
        {
            public Counter(CounterCapacities capacity)
            {
                this.Capacity = capacity;

                //// Capture
                this.Capture = new mtc::MetroWindow()
                {
                    Height = HeightSet[(int)this.Capacity],
                    Width = WidthSet[(int)this.Capacity],
                    Title = string.Format("bayoen-star-{0}", PlayerSet[(int)this.Capacity]),
                    BorderThickness = new Thickness(0),
                    ResizeMode = ResizeMode.NoResize,
                    ShowIconOnTitleBar = false,
                    WindowTitleBrush = Brushes.Transparent,
                    TitleCharacterCasing = CharacterCasing.Normal,
                };

                // Capture.Display
                this.CaptureDisplay = new CounterDisplay(CounterDisplayTypes.Capture);
                this.Capture.Content = this.CaptureDisplay;

                // Capture.Commands
                SetCaptureCommands();
                this.Capture.RightWindowCommands = this.Commands;

                // Capture.Flyout
                SetCaptureFlyout();
                this.Capture.Flyouts = this.FlyoutsControl;

                // Capture.Functions
                this.SetBasicFunctions(this.Capture);
                this.Capture.Hide();
                

                //// Overlay
                this.OverlayDisplay = new CounterDisplay(CounterDisplayTypes.Overlay);
                this.Overlay = new mtc::MetroWindow()
                {
                    Height = HeightSet[(int)this.Capacity],
                    Width = WidthSet[(int)this.Capacity],
                    Title = string.Format("bayoen-star-overlay{0}", PlayerSet[(int)this.Capacity]),
                    Content = this.OverlayDisplay,
                };
                this.SetBasicFunctions(this.Overlay);
                this.Overlay.Hide();

            }

            // Windows
            public mtc::MetroWindow Capture;

            public mtc::MetroWindow Overlay;

            //// Contents
            // Capture
            public CounterDisplay CaptureDisplay;
            public WindowCommands Commands;
            public TextBlock StatusTextBlock;
            public ContextMenu MenuContextMenu;
            public MenuItem ResetMenuItem;
            public mtc::FlyoutsControl FlyoutsControl;
            public mtc::Flyout GoalFlyout;
            public StackPanel GoalFlyoutPanel;
            public ComboBox GoalTypeComboBox;
            public mtc::NumericUpDown GoalScoreNumericUpDown;
            public Button SetGoalButton;
            public Button ClearGoalButton;

            public CounterDisplay OverlayDisplay;


            // Values
            public CounterCapacities Capacity { get; }

            private string _status;
            public string Status
            {
                get => this._status;
                set
                {
                    //


                    this._status = value;
                }
            }

            private DisplayModes _mode;
            public DisplayModes Mode
            {
                get => this._mode;
                set
                {
                    //


                    this._mode = value;
                }
            }

            private GoalTypes _goalType;
            public GoalTypes GoalType
            {
                get => this._goalType;
                set
                {

                    this._goalType = value;
                }
            }

            private int _goalScore;
            public int GoalScore
            {
                get => this._goalScore;
                set
                {

                    this._goalScore = value;
                }
            }


            // Parameters
            public readonly List<int> PlayerSet = new List<int>(2) { 2, 4 };
            public readonly List<double> HeightSet = new List<double>(2) { 160, 160 };
            public readonly List<double> WidthSet = new List<double>(2) { 525, 1300 };
            public readonly List<double> MonitorSet = new List<double>(2) { 600, 525 };
            public readonly List<int> AxesX = new List<int>(4) { 0, 418, 838, 1256 };

            private bool SetBasicFunctions(Window window)
            {
                window.MouseLeftButtonDown += (sender, e) =>
                {
                    (sender as mtc::MetroWindow).DragMove();
                };
                window.Closing += (sender, e) =>
                {
                    e.Cancel = true;
                    (sender as mtc::MetroWindow).Hide();
                };

                return true;
            }

            private void SetCaptureCommands()
            {
                this.Commands = new WindowCommands();

                this.StatusTextBlock = new TextBlock()
                {
                    FontSize = 12,
                    Margin = new Thickness(8, 3, 8, 3),
                    Foreground = Brushes.DimGray,
                    IsEnabled = false,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                this.Commands.Items.Add(this.StatusTextBlock);

                this.MenuContextMenu = new ContextMenu();
                #region MenuContextMenu
                this.ResetMenuItem = BuildResourceMenuItem("Reset", "appbar_new");
                //ResetMenuItem.Click += ResetMenuItem_ClickAsync;
                MenuContextMenu.Items.Add(ResetMenuItem);

                MenuItem GoalMenuItem = BuildResourceMenuItem("Goal", "appbar_controller_xbox");
                MenuContextMenu.Items.Add(GoalMenuItem);

                List<MenuItem> GoalItems = new List<MenuItem>();
                MenuItem SetGoalItem = new MenuItem()
                {
                    Header = "Set...",
                    ToolTip = "Set goal with its score and type; 목표를 입력하고 설정합니다",
                };
                SetGoalItem.Click += (sender, e) =>
                {
                    this.GoalFlyout.IsOpen = true;
                };
                GoalItems.Add(SetGoalItem);
                List<Tuple<GoalTypes, System.Drawing.Bitmap>> GoalTypeSets = new List<Tuple<GoalTypes, System.Drawing.Bitmap>>()
                    {
                        new Tuple<GoalTypes, System.Drawing.Bitmap>( GoalTypes.None,  null),
                        new Tuple<GoalTypes, System.Drawing.Bitmap>( GoalTypes.Star, bayoen.Properties.Resources.StarPlus),
                        new Tuple<GoalTypes, System.Drawing.Bitmap>( GoalTypes.Crown, bayoen.Properties.Resources.CrownLight),
                    };
                List<ComboBoxItem> GoalTypeItemList = new List<ComboBoxItem>();
                foreach (Tuple<GoalTypes, System.Drawing.Bitmap> tokenGoalType in GoalTypeSets)
                {
                    StackPanel TokenGoalTypeStackPanel = new StackPanel()
                    {
                        Margin = new Thickness(0),
                        Orientation = Orientation.Horizontal,
                    };

                    Image TokenGoalTypeImage = new Image()
                    {
                        Width = 14,
                        Height = 14,
                        Margin = new Thickness(3, 2, 5, 2),
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    if (tokenGoalType.Item2 != null) TokenGoalTypeImage.SetBitmap(tokenGoalType.Item2);
                    TokenGoalTypeStackPanel.Children.Add(TokenGoalTypeImage);

                    TextBlock TokenGoalTypeTextBlock = new TextBlock()
                    {
                        Text = tokenGoalType.Item1.ToString(),
                        FontWeight = FontWeights.Normal,
                        Margin = new Thickness(2),
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    TokenGoalTypeStackPanel.Children.Add(TokenGoalTypeTextBlock);

                    ComboBoxItem TokenGoalTypeItem = new ComboBoxItem()
                    {
                        Background = Brushes.Black,
                        Content = TokenGoalTypeStackPanel,
                    };
                    GoalTypeItemList.Add(TokenGoalTypeItem);
                }
                //this.GoalTypeComboBox.ItemsSource = GoalTypeItemList;
                MenuItem RemoveGoalItem = new MenuItem()
                {
                    Header = "Remove",
                    ToolTip = "Remove goal; 목표를 취소합니다",
                };
                RemoveGoalItem.Click += (sender, e) =>
                {
                    //this.preferences.GoalType = GoalTypes.None;
                    //this.preferences.GoalScore = 0;

                    //this.GoalType = this.preferences.GoalType.Value;
                    //this.GoalScore = this.preferences.GoalScore.Value;
                };
                GoalItems.Add(RemoveGoalItem);
                GoalMenuItem.ItemsSource = GoalItems;

                //if (this.preferences.GoalType == null)
                //{
                //    this.preferences.GoalType = GoalTypes.None;
                //}
                //if (this.preferences.GoalScore == null)
                //{
                //    this.preferences.GoalScore = 0;
                //}

                //this.GoalTypeComboBox.SelectedIndex = (int)this.preferences.GoalType.Value;
                //this.GoalScoreNumericUpDown.Value = this.preferences.GoalScore.Value;

                MenuItem OverlayMenuItem = BuildResourceMenuItem("Overlay", "appbar_app_plus");
                //OverlayMenuItem.Click += OverlayMenuItem_Click;
                MenuContextMenu.Items.Add(OverlayMenuItem);

                MenuItem SettingMenuItem = BuildResourceMenuItem("Settings", "appbar_settings");
                //SettingMenuItem.Click += SettingMenuItem_Click;
                MenuContextMenu.Items.Add(SettingMenuItem);

                MenuItem ModeMenuItem = BuildResourceMenuItem("Mode", "appbar_list");
                MenuContextMenu.Items.Add(ModeMenuItem);

                List<MenuItem> ModeItems = new List<MenuItem>();
                MenuItem Mode1Item = new MenuItem()
                {
                    Header = "1. Star+",
                    ToolTip = "Count STARs and display (hidden GAMEs); 별을 세어 보여줍니다 (게임은 숨깁니다)",
                    IsCheckable = true,
                };
                Mode1Item.Click += (sender, e) =>
                {
                    this.Mode = DisplayModes.Star_plus_only;
                    //this.CheckContainers();

                    //this.preferences.DisplayMode = this.Mode;
                    ModeItems.ForEach(x => x.IsChecked = false);
                    (sender as MenuItem).IsChecked = true;
                };
                ModeItems.Add(Mode1Item);

                MenuItem Mode2Item = new MenuItem()
                {
                    Header = "2. Game",
                    ToolTip = "Count GAMEs and display (hidden STARs); 게임을 세어 보여줍니다 (별은 숨깁니다)",
                    IsCheckable = true,
                };
                Mode2Item.Click += (sender, e) =>
                {
                    this.Mode = DisplayModes.Game_only;
                    //this.CheckContainers();

                    //this.preferences.DisplayMode = this.Mode;
                    ModeItems.ForEach(x => x.IsChecked = false);
                    (sender as MenuItem).IsChecked = true;
                };
                ModeItems.Add(Mode2Item);

                MenuItem Mode3Item = new MenuItem()
                {
                    Header = "3. Game & Star",
                    ToolTip = "Count only GAMEs not STARs just display; 게임만 세고 별은 그대로 보여줍니다",
                    IsCheckable = true,
                };
                Mode3Item.Click += (sender, e) =>
                {
                    this.Mode = DisplayModes.Game_and_Star;
                    //this.CheckContainers();

                    //this.preferences.DisplayMode = this.Mode;
                    ModeItems.ForEach(x => x.IsChecked = false);
                    (sender as MenuItem).IsChecked = true;
                };
                ModeItems.Add(Mode3Item);

                MenuItem Mode4Item = new MenuItem()
                {
                    Header = "4. Game & Star+",
                    ToolTip = "Count/Display both GAMEs and STARs; 게임과 별을 함께 세어 보여줍니다",
                    IsCheckable = true,
                };
                Mode4Item.Click += (sender, e) =>
                {
                    this.Mode = DisplayModes.Game_and_Star_plus;
                    //this.CheckContainers();

                    //this.preferences.DisplayMode = this.Mode;
                    ModeItems.ForEach(x => x.IsChecked = false);
                    (sender as MenuItem).IsChecked = true;
                };
                ModeItems.Add(Mode4Item);

                MenuItem Mode5Item = new MenuItem()
                {
                    Header = "5. Star & Star+",
                    ToolTip = "Couont/Display STARs (hidden GAMEs); 별을 세고 현재 별을 보여줍니다 (게임은 숨깁니다)",
                    IsCheckable = true,
                };
                Mode5Item.Click += (sender, e) =>
                {
                    this.Mode = DisplayModes.Star_plus_and_Star;
                    //this.CheckContainers();

                    //this.preferences.DisplayMode = this.Mode;
                    ModeItems.ForEach(x => x.IsChecked = false);
                    (sender as MenuItem).IsChecked = true;
                };
                ModeItems.Add(Mode5Item);

                ModeMenuItem.ItemsSource = ModeItems;

                //if (this.preferences.DisplayMode == null)
                //{
                //    this.preferences.DisplayMode = DisplayModes.Game_and_Star_plus;
                //    (ModeItems[0] as MenuItem).IsChecked = true;
                //}
                //else
                //{
                //    int modeIndex = (int)this.preferences.DisplayMode.Value;
                //    (ModeItems[modeIndex] as MenuItem).IsChecked = true;
                //}

                StackPanel MenuButtonPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };

                Rectangle MenuButtonRectangle = BuildResourceRectangle("appbar_list_gear");
                MenuButtonRectangle.Height = 15;
                MenuButtonRectangle.Width = 15;
                MenuButtonRectangle.Margin = new Thickness(3, 6, 5, 3);
                MenuButtonRectangle.Fill = Brushes.White;
                MenuButtonPanel.Children.Add(MenuButtonRectangle);

                MenuButtonPanel.Children.Add(new TextBlock()
                {
                    Text = "Menu",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Left,
                    Margin = new Thickness(3, 3, 3, 3),
                    VerticalAlignment = VerticalAlignment.Center,
                });

                Button MenuButton = new Button()
                {
                    Background = Brushes.Transparent,
                    ToolTip = "Hotkey 'M'",
                    Content = MenuButtonPanel,
                    ContextMenu = MenuContextMenu,
                };
                MenuButton.Click += (sender, e) =>
                {
                    (sender as Button).ContextMenu.IsOpen = true;
                };
                #endregion
                this.Commands.Items.Add(MenuButton);

                this.MenuContextMenu.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.O)
                    {
                        //this.ViewOverlay();
                        this.MenuContextMenu.IsOpen = false;
                    }
                    else if (e.Key == Key.R)
                    {
                        //this.Reset();
                        this.MenuContextMenu.IsOpen = false;
                    }
                };

                this.Capture.KeyDown += (sender, e) =>
                {
                    if (e.Key == Key.M)
                    {
                        this.MenuContextMenu.IsOpen = true;
                    }
                };
            }

            private void SetCaptureFlyout()
            {
                this.FlyoutsControl = new FlyoutsControl()
                {

                };

                this.GoalFlyoutPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                this.GoalFlyoutPanel.Children.Add(new TextBlock()
                {
                    Text = "Type",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Height = 15,
                    Margin = new Thickness(3, 6, 3, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                });

                this.GoalTypeComboBox = new ComboBox()
                {
                    Height = 15,
                    Width = 100,
                    Margin = new Thickness(3, 6, 6, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                this.GoalFlyoutPanel.Children.Add(this.GoalTypeComboBox);

                this.GoalFlyoutPanel.Children.Add(new TextBlock()
                {
                    Text = "Score",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Height = 15,
                    Margin = new Thickness(3, 6, 3, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                });

                this.GoalScoreNumericUpDown = new NumericUpDown()
                {
                    Minimum = 0,
                    Maximum = 9999,
                    Value = 0,
                    Height = 15,
                    Width = 100,
                    Margin = new Thickness(3, 6, 6, 6),
                };
                this.GoalFlyoutPanel.Children.Add(this.GoalScoreNumericUpDown);

                this.SetGoalButton = new Button()
                {
                    Content = "Set",
                    Height = 15,
                    Width = 50,
                    Margin = new Thickness(3, 6, 3, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                this.SetGoalButton.SetValue(mtc::ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
                this.SetGoalButton.SetResourceReference(Control.StyleProperty, "AccentedSquareButtonStyle");
                this.SetGoalButton.Click += (sender, e) =>
                {
                    this.GoalType = (GoalTypes)(this.GoalTypeComboBox.SelectedIndex);
                    this.GoalScore = (int)this.GoalScoreNumericUpDown.Value.Value;

                    //this.preferences.GoalType = this.GoalType;
                    //this.preferences.GoalScore = this.GoalScore;

                    
                    //System.Media.SystemSounds.Hand.Play();

                    this.GoalFlyout.IsOpen = false;
                };
                this.GoalFlyoutPanel.Children.Add(this.SetGoalButton);

                this.ClearGoalButton = new Button()
                {
                    Content = "Clear",
                    Height = 15,
                    Width = 50,
                    Margin = new Thickness(3, 6, 3, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                this.ClearGoalButton.SetValue(mtc::ControlsHelper.ContentCharacterCasingProperty, CharacterCasing.Normal);
                this.ClearGoalButton.SetResourceReference(Control.StyleProperty, "SquareButtonStyle");
                this.ClearGoalButton.Click += (sender, e) =>
                {
                    //this.GoalTypeComboBox.SelectedIndex = 0;
                    //this.GoalScoreNumericUpDown.Value = 0;

                    this.GoalFlyout.IsOpen = false;
                };
                this.GoalFlyoutPanel.Children.Add(this.ClearGoalButton);              

                this.GoalFlyout = new Flyout()
                {
                    Header = "Set Goal...",
                    Position = Position.Right,
                    Width = this.Capture.Width,
                    Content = this.GoalFlyoutPanel,
                };
                FlyoutsControl.Items.Add(this.GoalFlyout);

            }

            public bool SetValue()
            {
                return true;
            }

            public MenuItem BuildResourceMenuItem(string header, string appbar)
            {
                return new MenuItem()
                {
                    Header = header,
                    Icon = new Rectangle()
                    {
                        Width = 15,
                        Height = 15,
                        Fill = Brushes.White,
                        Margin = new Thickness(8, 8, 2, 8),
                        OpacityMask = new VisualBrush()
                        {
                            AlignmentX = AlignmentX.Center,
                            AlignmentY = AlignmentY.Center,
                            Stretch = Stretch.Uniform,
                            Visual = Application.Current.TryFindResource(appbar) as Visual,
                        },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    },
                };
            }

            public Rectangle BuildResourceRectangle(string appbar)
            {
                Rectangle TokenRectangle = new Rectangle()
                {
                    OpacityMask = new VisualBrush()
                    {
                        AlignmentX = AlignmentX.Center,
                        AlignmentY = AlignmentY.Center,
                        Stretch = Stretch.Fill,
                        Visual = Application.Current.TryFindResource(appbar) as Visual,
                    },
                };

                return TokenRectangle;
            }
        }

        public class CounterDisplay : Grid
        {
            public CounterDisplay(CounterDisplayTypes type)
            {

            }


        }

        public enum CounterCapacities : int
        {
            TwoMax = 0,
            FourMax = 1,
        }

        public enum CounterDisplayTypes : int
        {
            Capture = 0,
            Overlay = 1,
            Monitor = 2,
        }
    }
}



