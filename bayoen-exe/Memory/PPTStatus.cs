using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bayoen.Data;

namespace bayoen.Memory
{
    public class PPTStatus : ICloneable
    {
        public PPTStatus()
        {
            
        }

        public PPTMainStates MainState { get; private set; }
        public PPTSubStates SubState { get; private set; }
        public PPTGameModes GameMode { get; private set; }
        public bool IsEndurance { get; private set; }
        public int LobbySize { get; private set; }
        public int LobbyMax { get; private set; }
        public int GameFrame { get; private set; }
        public int SceneFrame { get; private set; }

        public int MyRating { get; private set; }

        public List<int> PlayerStars { get; private set; }

        public bool PuzzleLeagueGameFinishFlag { get; private set; }

        public bool Check()
        {
            if (!Core.PPTMemory.CheckProcess())
            {
                this.MainState = PPTMainStates.Offline;
                return false;
            }

            int? menuID = Core.PPTMemory.MenuIndex;
            if (menuID.HasValue)
            {
                this.MainState = GetMainState(menuID.Value);
                this.SubState = GetSubState(menuID.Value);

                this.LobbySize = Core.PPTMemory.LobbySize;
                this.LobbyMax = Core.PPTMemory.LobbyMax;
                //if (menuID == 28)
                //{
                //    // Freeplay
                //    this.LobbySize = Core.PPTMemory.LobbySize;
                //    this.LobbyMax = Core.PPTMemory.LobbyMax;
                //}
                //else
                //{
                //    // Otherwise (ignore acrade)
                //    this.LobbySize = this.LobbyMax = -1;
                //}
                return true;
            }

            if (Core.PPTMemory.InAdventure)
            {
                this.MainState = PPTMainStates.Adventure;
                this.SubState = PPTSubStates.Empty;
                return true;
            }

            if (Core.PPTMemory.InInitial)
            {
                this.MainState = PPTMainStates.Title;
                this.SubState = PPTSubStates.Empty;
                return true;
            }

            if (Core.PPTMemory.InOnlineReplay)
            {
                if (Core.PPTMemory.InLocalReplay) this.MainState = PPTMainStates.LocalReplay;
                else this.MainState = PPTMainStates.OnlineReplay;
                this.SubState = PPTSubStates.Empty;
                return true;
            }

            int mainID = Core.PPTMemory.MainIDFromFlag;
            int modeID = Core.PPTMemory.Mode(mainID);
            this.MainState = GetMainStateFromFlag(mainID);
            this.GameMode = GetGameMode(modeID);
            this.IsEndurance = IsEnduranceGame(modeID);

            if (Core.PPTMemory.InCharacterSelection)
            {
                this.SubState = PPTSubStates.CharacterSelection;
                return true;
            }

            if (Core.PPTMemory.InReady)
            {
                this.SubState = PPTSubStates.InReady;
                return true;
            }
            
            if (Core.PPTMemory.InMatch)
            {
                this.SubState = PPTSubStates.InMatch;
                this.GameFrame = Core.PPTMemory.GameFrame;
                this.SceneFrame = Core.PPTMemory.SceneFrame;
                this.PuzzleLeagueGameFinishFlag = Core.PPTMemory.PuzzleLeagueGameFinishFlag;
                this.PlayerStars = Core.PPTMemory.PlayerStars;
                this.MyRating = Core.PPTMemory.MyRating;

                return true;
            }

            return true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        private static PPTMainStates GetMainState(int id)
        {
            switch (id)
            {
                case 0:
                case 12: return PPTMainStates.Loading;
                case 1: return PPTMainStates.MainMenu;
                case 2: return PPTMainStates.Adventure;
                case 3:
                case 18: return PPTMainStates.SoloArcade;
                case 4: return PPTMainStates.MultiArcade;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9: return PPTMainStates.Option;
                case 10:
                case 32:
                case 33:
                case 37: return PPTMainStates.Online;
                case 11: return PPTMainStates.Lessons;
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 19: return PPTMainStates.CharacterSetection;
                case 20:
                case 25:
                case 27:
                case 34: return PPTMainStates.PuzzleLeague;
                case 21:
                case 23:
                case 24:
                case 26:
                case 28: return PPTMainStates.FreePlay;

            }
            return PPTMainStates.Invalid;
        }

        private static PPTMainStates GetMainStateFromFlag(int id)
        {
            switch (id)
            {
                case 0: return PPTMainStates.Adventure;
                case 1: return PPTMainStates.SoloArcade;
                case 2: return PPTMainStates.MultiArcade;
                case 3: return PPTMainStates.Option;
                case 4:
                    switch (Core.PPTMemory.OnlineType)
                    {
                        case 0: return PPTMainStates.PuzzleLeague;
                        case 1: return PPTMainStates.FreePlay;
                        default: return PPTMainStates.None;
                    }                    
                case 5: return PPTMainStates.Lessons;
            }
            return PPTMainStates.None;
        }

        private static PPTSubStates GetSubState(int id)
        {
            switch (id)
            {
                case 3:
                case 4:
                case 23: return PPTSubStates.ModeSelect;
                case 6: return PPTSubStates.Stats;
                case 7: return PPTSubStates.Options;
                case 8: return PPTSubStates.Theatre;
                case 9: return PPTSubStates.Shop;
                case 18: return PPTSubStates.ChallengeModeSelection;
                case 20: return PPTSubStates.Standby;
                case 21: return PPTSubStates.RoomSelection;
                case 24: return PPTSubStates.RoomCreation;
                case 25:
                case 27: return PPTSubStates.Matchmaking;
                case 26:
                case 28: return PPTSubStates.InLobby;
                case 32: return PPTSubStates.Replays;
                case 33:
                case 37: return PPTSubStates.ReplayUpload;
                case 34: return PPTSubStates.Rankings;
            }
            return PPTSubStates.Empty;
        }

        private static PPTGameModes GetGameMode(int id)
        {
            switch (id)
            {
                case 0:
                case 5: return PPTGameModes.Versus;
                case 1:
                case 6: return PPTGameModes.Fusion;
                case 2:
                case 7: return PPTGameModes.Swap;
                case 3:
                case 8: return PPTGameModes.Party;
                case 4:
                case 9: return PPTGameModes.BigBang;                
                case 10: return PPTGameModes.EndlessFever;
                case 11: return PPTGameModes.TinyPuyo;
                case 12: return PPTGameModes.EndlessPuyo;
                case 13: return PPTGameModes.Sprint;
                case 14: return PPTGameModes.Marathon;
                case 15: return PPTGameModes.Ultra;
            }

            return PPTGameModes.None;
        }

        private static bool IsEnduranceGame(int id)
        {
            return (4 < id) && (id < 10);
        }
    }
}
