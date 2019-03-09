using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using bayoen;
using bayoen.Data;

namespace bayoen.Memory
{
    public class PPTTimer : DispatcherTimer
    {
        public PPTTimer(TimeSpan span)
        {
            this.Interval = span;
            this.Tick += PPTTimer_Tick;
        }

        private void PPTTimer_Tick(object sender, EventArgs e)
        {            
            if (!Core.PPTStatus.Check()) return;

            this.PuzzleLeagueTick();

            //Core.MainWindow.TextOut.Text = Core.CurrentMatch.ToJSON().ToString();

            Core.OldPPTStatus = Core.PPTStatus.Clone() as PPTStatus;

#if DEBUG
            Core.DebugWindow.TextOut.Text = $"MainState: {Core.PPTStatus.MainState.ToString()}"
                + $"\nSubState: {(Core.PPTStatus.SubState == PPTSubStates.Empty ? "" : Core.PPTStatus.SubState.ToString())}"
                + $"\nGameMode: {(Core.PPTStatus.GameMode == PPTGameModes.None ? "" : Core.PPTStatus.GameMode.ToString())}{(Core.PPTStatus.IsEndurance ? " (Endurance)" : "")}"
                + $"\nMyRating: {Core.PPTMemory.MyRating}";
#endif

            #region [text dashboard]
            if (true) // Core.PPTMemory.CheckProcess()
            {
                // Do scan PPT



            }
            #endregion
        }

        private void PuzzleLeagueTick()
        {
            if (Core.PPTStatus.MainState == PPTMainStates.PuzzleLeague)
            {
                if (Core.PPTStatus.SubState == PPTSubStates.InMatch)
                {
                    if (Core.OldPPTStatus.SubState == PPTSubStates.CharacterSelection)
                    {
                        // New match
                        Core.CurrentMatch.Initialize();
                    }
                    else if (Core.OldPPTStatus.SubState == PPTSubStates.InMatch)
                    {
                        // In match
                        if ((Core.PPTStatus.GameFrame > 0 && Core.OldPPTStatus.GameFrame == 0) // 0 -> 1 game
                            || (Core.PPTStatus.GameFrame < Core.OldPPTStatus.GameFrame)) // n-1 -> n game
                        {
                            #region [ Draw game and save it ]
                            // Save draw game (post processing)
                            if (Core.CurrentGame.GameEnd != DateTime.MinValue && Core.CurrentGame.Winners.Count == 0)
                            {
                                if (Core.PPTStatus.PlayerStars.SequenceEqual(Core.OldPPTStatus.PlayerStars))
                                {
                                    Core.CurrentMatch.SaveCurrentGame();
                                }
                            }
                            #endregion

                            // New game
                            Core.CurrentGame.Initialize();
                            return;
                        }

                        // record ticks
                        Core.CurrentGame.CheckTickScores();

                        if (Core.PPTStatus.PuzzleLeagueGameFinishFlag && !Core.OldPPTStatus.PuzzleLeagueGameFinishFlag)
                        {
                            if (Core.OldPPTStatus.GameFrame > 0 && Core.CurrentGame.GameEnd == DateTime.MinValue)
                            {
                                // Terminate game 1
                                Core.CurrentGame.End();
                            }
                        }

                        if (!Core.PPTStatus.PlayerStars.SequenceEqual(Core.OldPPTStatus.PlayerStars))
                        {
                            // Terminate game 2
                            List<int> diff = Core.PPTStatus.PlayerStars.Zip(Core.OldPPTStatus.PlayerStars, (x, y) => x - y).ToList();
                            if (Core.PPTStatus.LobbyMax == 2)
                            {
                                diff.RemoveRange(2, 2);
                                if (Core.PPTMemory.MyIndex == 1) diff.Reverse();
                            }

                            for (int playerIndex = 0; playerIndex < diff.Count; playerIndex++)
                            {
                                if (diff[playerIndex] == 1) Core.CurrentGame.Winners.Add(playerIndex + 1);
                            }

                            Core.CurrentMatch.SaveCurrentGame();
                        }

                        if (Core.CurrentMatch.WinCount > 0)
                        {
                            if (Core.CurrentMatch.MatchEnd == DateTime.MinValue)
                            {
                                if (Core.CurrentMatch.MatchWins().IndexOf(Core.CurrentMatch.WinCount) > -1)
                                {
                                    Core.CurrentMatch.End();
                                }
                            }
                        }

                        if (Core.PPTStatus.MyRating != Core.OldPPTStatus.MyRating)
                        {                            
                            if (Core.CurrentMatch.MatchEnd != DateTime.MinValue)
                            {
                                Core.CurrentMatch.GetRatingGain();
                            }
                            else
                            {
                                Core.CurrentMatch.SaveCurrentGame();
                                Core.CurrentMatch.Crashed();
                            }
                            
                            if (!Directory.Exists(Config.StatFolderName)) Directory.CreateDirectory(Config.StatFolderName);
                            Core.CurrentMatch.Save(Path.Combine(Config.StatFolderName, $"match_s{Core.PPTMemory.MySteamID32}_t{Core.CurrentMatch.MatchBegin.ToString("yyMMdd_hhmmss")}.json"));

                            Core.MainWindow.HomeTabGrid.RecentNavigator.CheckGrid();
                        }                        
                    }
                }
            }
        }
    }
}
