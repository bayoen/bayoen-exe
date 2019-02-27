using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using bayoen;

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
            

            Core.OldPPTStatus = Core.PPTStatus.Clone() as PPTStatus;
            
            #region [text dashboard]
            if (false) // Core.PPTMemory.CheckProcess()
            {
               // Do scan PPT
                Core.MainWindow.TextOut.Text = $"{Core.PPTStatus.MainState.ToString()}"
                    + $"\n{(Core.PPTStatus.SubState == PPTSubStates.Empty ? "" : Core.PPTStatus.SubState.ToString())}"
                    + $"\n{(Core.PPTStatus.GameMode == PPTGameModes.None ? "" : Core.PPTStatus.GameMode.ToString())}{(Core.PPTStatus.IsEndurance ? " (Endurance)" : "")}"
                    + $"\n\nGameFrame: {Core.PPTMemory.GameFrame}"
                    + $"\nSceneFrame: {Core.PPTMemory.SceneFrame}"
                    + $"\nStar: {Core.PPTMemory.PlayerStar(0)}-{Core.PPTMemory.PlayerStar(1)}-{Core.PPTMemory.PlayerStar(2)}-{Core.PPTMemory.PlayerStar(3)}"
                    + $"\nName: {Core.PPTMemory.PlayerNameForced(0)} - {Core.PPTMemory.PlayerNameForced(1)} - {Core.PPTMemory.PlayerNameForced(2)} - {Core.PPTMemory.PlayerNameForced(3)}"
                    + $"\nScore: {string.Join(" - ", Core.PPTMemory.PlayerScores)}"
                    + $"\nRating: {Core.PPTMemory.PlayerRatingForced(0)} - {Core.PPTMemory.PlayerRatingForced(1)} - {Core.PPTMemory.PlayerRatingForced(2)} - {Core.PPTMemory.PlayerRatingForced(3)}"
                    + $"\nSteam: {Core.PPTMemory.PlayerSteamID32Forced(0)} - {Core.PPTMemory.PlayerSteamID32Forced(1)} - {Core.PPTMemory.PlayerSteamID32Forced(2)} - {Core.PPTMemory.PlayerSteamID32Forced(3)}";

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
                        Core.MainWindow.TextOut.Text += "\n\n";
                        Core.MainWindow.TextOut.Text += $"{DateTime.Now.ToString()}: {Core.PPTMemory.PlayerNameForced(0)} vs. {Core.PPTMemory.PlayerNameForced(1)}";
                    }
                    else if (Core.OldPPTStatus.SubState == PPTSubStates.InMatch)
                    {
                        if ((Core.PPTStatus.GameFrame > 0 && Core.OldPPTStatus.GameFrame == 0)
                            || (Core.PPTStatus.GameFrame < Core.OldPPTStatus.GameFrame))
                        {
                            if (Core.CurrentGame.GameEnd != DateTime.MinValue && Core.CurrentGame.Winners.Count == 0)
                            {
                                if (Core.PPTStatus.PlayerStars.SequenceEqual(Core.OldPPTStatus.PlayerStars))
                                {
                                    Core.MainWindow.TextOut.Text += " [Draw]";                                    
                                    Core.CurrentMatch.SaveCurrentGame();
                                }
                            }                            

                            // New game
                            Core.CurrentGame.Initialize();
                            Core.MainWindow.TextOut.Text += $"\nGame {(Core.CurrentMatch.Games.Count + 1).ToString()}: {Core.CurrentGame.GameBegin.ToString("h:mm:ss")}";
                            return;
                        }

                        //// In game
                        // Do nothing yet

                        if (Core.PPTStatus.PuzzleLeagueGameFinishFlag && !Core.OldPPTStatus.PuzzleLeagueGameFinishFlag)
                        {
                            if (Core.OldPPTStatus.GameFrame > 0 && Core.CurrentGame.GameEnd == DateTime.MinValue)
                            {
                                // Terminate game 1
                                Core.CurrentGame.End();
                                Core.MainWindow.TextOut.Text += $" - {Core.CurrentGame.GameEnd.ToString("h:mm:ss")}";
                            }
                        }

                        if (!Core.PPTStatus.PlayerStars.SequenceEqual(Core.OldPPTStatus.PlayerStars))
                        {
                            // Terminate game 1
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

                            Core.MainWindow.TextOut.Text += $" [{Core.CurrentGame.Winners.Single().ToString()}P Won]";
                            Core.CurrentMatch.SaveCurrentGame();
                        }
                    }
                }
            }
        }
    }
}
