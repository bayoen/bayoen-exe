using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bayoen.Data;
using bayoen.Data.Enums;

namespace bayoen.Memory
{
    public class PPTStatus : ICloneable
    {
        public PPTStatus()
        {
            
        }

        public MainStates MainState { get; private set; }
        public SubStates SubState { get; private set; }
        public GameModes GameMode { get; private set; }
        public bool IsEndurance { get; private set; }
        public int LobbySize { get; private set; }
        public int LobbyMax { get; private set; }
        public int GameFrame { get; private set; }
        public int SceneFrame { get; private set; }

        public int MyRating { get; private set; }

        public List<int> PlayerStars { get; private set; }

        public bool IsGameFinished { get; private set; }

        public bool Check()
        {
            if (!Core.PPTMemory.Check())
            {
                this.MainState = MainStates.Offline;
                return false;
            }

            int? menuID = Core.PPTMemory.MenuIndex;
            if (menuID.HasValue)
            {
                this.MainState = GetMainState(menuID.Value);
                this.SubState = GetSubState(menuID.Value);

                this.LobbySize = Core.PPTMemory.LobbySize;
                this.LobbyMax = Core.PPTMemory.LobbyMax;
                return true;
            }

            if (Core.PPTMemory.InAdventure)
            {
                this.MainState = MainStates.Adventure;
                this.SubState = SubStates.Empty;
                return true;
            }

            if (Core.PPTMemory.InInitial)
            {
                this.MainState = MainStates.Title;
                this.SubState = SubStates.Empty;
                return true;
            }

            if (Core.PPTMemory.InOnlineReplay)
            {
                if (Core.PPTMemory.InLocalReplay) this.MainState = MainStates.LocalReplay;
                else this.MainState = MainStates.OnlineReplay;
                this.SubState = SubStates.Empty;
                return true;
            }

            int mainID = Core.PPTMemory.MainIDFromFlag;
            int modeID = Core.PPTMemory.Mode(mainID);
            this.MainState = GetMainStateFromFlag(mainID);
            this.GameMode = GetGameMode(modeID);
            this.IsEndurance = IsEnduranceGame(modeID);

            if (Core.PPTMemory.InCharacterSelection)
            {
                this.SubState = SubStates.CharacterSelection;
                return true;
            }

            if (Core.PPTMemory.InReady)
            {
                this.SubState = SubStates.InReady;
                return true;
            }

            if (Core.PPTMemory.InMatch)
            {
                this.SubState = SubStates.InMatch;
                this.GameFrame = Core.PPTMemory.GameFrame;
                this.SceneFrame = Core.PPTMemory.SceneFrame;
                this.IsGameFinished = Core.PPTMemory.IsGameFinished;
                this.PlayerStars = Core.PPTMemory.PlayerStars;
                this.MyRating = Core.PPTMemory.MyRating;
            }

            if (this.MainState == MainStates.SoloArcade || this.MainState == MainStates.MultiArcade)
            {
                if (Core.PPTMemory.PlayerNameLocal(0) + Core.PPTMemory.PlayerNameLocal(1) == "")
                {
                    this.MainState = MainStates.Demo;
                }
            }

            return true;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        private static MainStates GetMainState(int id)
        {
            switch (id)
            {
                case 0:
                case 12: return MainStates.Loading;
                case 1: return MainStates.MainMenu;
                case 2: return MainStates.Adventure;
                case 3:
                case 18: return MainStates.SoloArcade;
                case 4: return MainStates.MultiArcade;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9: return MainStates.Option;
                case 10:
                case 32:
                case 33:
                case 37: return MainStates.Online;
                case 11: return MainStates.Lessons;
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 19: return MainStates.CharacterSetection;
                case 20:
                case 25:
                case 27:
                case 34: return MainStates.PuzzleLeague;
                case 21:
                case 23:
                case 24:
                case 26:
                case 28: return MainStates.FreePlay;

            }
            return MainStates.Invalid;
        }

        private static MainStates GetMainStateFromFlag(int id)
        {
            switch (id)
            {
                case 0: return MainStates.Adventure;
                case 1: return MainStates.SoloArcade;
                case 2: return MainStates.MultiArcade;
                case 3: return MainStates.Option;
                case 4:
                    switch (Core.PPTMemory.OnlineType)
                    {
                        case 0: return MainStates.PuzzleLeague;
                        case 1: return MainStates.FreePlay;
                        default: return MainStates.None;
                    }                    
                case 5: return MainStates.Lessons;
            }
            return MainStates.None;
        }

        private static SubStates GetSubState(int id)
        {
            switch (id)
            {
                case 3:
                case 4:
                case 23: return SubStates.ModeSelect;
                case 6: return SubStates.Stats;
                case 7: return SubStates.Options;
                case 8: return SubStates.Theatre;
                case 9: return SubStates.Shop;
                case 18: return SubStates.ChallengeModeSelection;
                case 20: return SubStates.Standby;
                case 21: return SubStates.RoomSelection;
                case 24: return SubStates.RoomCreation;
                case 25:
                case 27: return SubStates.Matchmaking;
                case 26:
                case 28: return SubStates.InLobby;
                case 32: return SubStates.Replays;
                case 33:
                case 37: return SubStates.ReplayUpload;
                case 34: return SubStates.Rankings;
            }
            return SubStates.Empty;
        }

        private static GameModes GetGameMode(int id)
        {
            switch (id)
            {
                case 0:
                case 5: return GameModes.Versus;
                case 1:
                case 6: return GameModes.Fusion;
                case 2:
                case 7: return GameModes.Swap;
                case 3:
                case 8: return GameModes.Party;
                case 4:
                case 9: return GameModes.BigBang;                
                case 10: return GameModes.EndlessFever;
                case 11: return GameModes.TinyPuyo;
                case 12: return GameModes.EndlessPuyo;
                case 13: return GameModes.Sprint;
                case 14: return GameModes.Marathon;
                case 15: return GameModes.Ultra;
            }

            return GameModes.None;
        }

        private static bool IsEnduranceGame(int id)
        {
            return (4 < id) && (id < 10);
        }
    }
}
