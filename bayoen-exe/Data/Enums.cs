using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bayoen.Data
{
    public enum PPTMainStates : int
    {
        Invalid,
        None,
        Offline,
        Title,

        MainMenu,
        Adventure,
        SoloArcade,
        MultiArcade,
        Option,
        Online,
        Lessons,

        Loading,
        CharacterSetection,

        PuzzleLeague,
        FreePlay,

        OnlineReplay,
        LocalReplay,
    }

    public enum PPTSubStates : int
    {
        Empty,
        ModeSelect,
        Stats,
        Options,
        Theatre,
        Shop,
        ChallengeModeSelection,
        CharacterSelection,
        Standby,
        RoomSelection,
        RoomCreation,
        Matchmaking,
        InLobby,
        InReady,
        InMatch,
        Replays,
        ReplayUpload,
        Rankings,
    }

    public enum PPTGameModes : int
    {
        None,
        Versus,
        Fusion,
        Swap,
        Party,
        BigBang,
        EndlessFever,
        TinyPuyo,
        EndlessPuyo,
        Sprint,
        Marathon,
        Ultra,
    }

    public enum PPTGameCategories : int
    {
        SoloArcade,
        MultiArcade,
        PuzzleLeague,
        FreePlay,
    }

    public enum PPTPlayTypes
    {
        PuyoPuyo,
        Tetris,
        Swap,
        Fusion,
    }

    public enum PPTRanks
    {
        PuyoPuyo,
        Tetris,
        Swap,
        Fusion,
    }

    public enum PPTLeagues
    {
        PuyoPuyo,
        Tetris,
        Swap,
        Fusion,
    }

    public enum PPTPlayRegion
    {
        PuyoPuyo,
        Tetris,
        Swap,
        Fusion,
    }

    public enum PPTMatchCrashes
    {
        None,
        ByMe,
        NotMe,
    }
}
