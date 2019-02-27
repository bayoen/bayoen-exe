﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bayoen.Memory
{
    public class PPTMemory : ProcessMemory
    {
        public PPTMemory(string processName) : base(processName, true)
        {

        }

        public int GameFrame => this.ReadInt32(new IntPtr(0x140461B20), 0x424);

        public int SceneFrame => this.ReadInt32(new IntPtr(0x140460C08), 0x18, 0x268, 0x140, 0x58);

        private IntPtr _scoreAddress;
        public IntPtr ScoreAddress => this._scoreAddress = new IntPtr(this.ReadInt32(new IntPtr(0x14057F048)) + 0x38);

        private IntPtr _playerAddress;
        public IntPtr PlayerAddress => this._playerAddress = new IntPtr(this.ReadInt32(new IntPtr(0x140473760), 0x20) + 0xD8);

        private IntPtr _leagueAddress;
        public IntPtr LeagueAddress => this._leagueAddress = new IntPtr(this.ReadInt32(new IntPtr(0x140473760), 0x68, 0x20, 0x970) - 0x38);

        public int WinCount => this.ReadInt32(this._scoreAddress + 0x10);
        public int WinCountForced => this.ReadInt32(this.ScoreAddress + 0x10);

        public int PlayerSteamID32(int index) => this.ReadInt32(this._playerAddress + index * 0x50 + 0x40);
        public int PlayerSteamID32Forced(int index) => this.ReadInt32(this.PlayerAddress + index * 0x50 + 0x40);

        public string PlayerName(int index) => this.ReadValidString(this._playerAddress + index * 0x50, Config.PlayerNameSize);
        public string PlayerNameForced(int index) => this.ReadValidString(this.PlayerAddress + index * 0x50, Config.PlayerNameSize);
        public string PlayerNameDirect(int index) => this.ReadValidString(new IntPtr(0x140598BD4 + index * 0x68), Config.PlayerNameSize);

        public int PlayerRating(int index) => this.ReadInt32(this._playerAddress + index * 0x50 + 0x30);
        public int PlayerRatingForced(int index) => this.ReadInt32(this.PlayerAddress + index * 0x50 + 0x30);

        //public int PlayerScore(int index) =>     this.ReadInt32(new IntPtr(0x140473760), 0x20, index* 0x50 + 0x118);
        //public int PlayerScoreForced(int index) => this.ReadInt32(new IntPtr(0x140473760), 0x20, index * 0x50 + 0x118);

        public int Player1Score => this.ReadInt32(new IntPtr(0x140461B28), 0x380, 0x18, 0xE0, 0x3C);
        public int Player2Score => this.ReadInt32(new IntPtr(0x140460690), 0x2D0, 0x0, 0x38, 0x78, 0xE0, 0x3C);
        public int Player3Score => -1;
        public int Player4Score => -1;
        public List<int> PlayerScores => new List<int>() { Player1Score, Player2Score, Player3Score, Player4Score };

        public int PlayerStar(int index)   => this.ReadInt32(new IntPtr(0x14057F048), index * 0x04 + 0x38);

        public List<int> PlayerStars => Enumerable.Range(0, 4).Select(x => PlayerStar(x)).ToList();

        public int LobbySize => this.ReadInt32(new IntPtr(0x140473760), 0x20, 0xB4);
        public int LobbyMax => this.ReadInt32(new IntPtr(0x140473760), 0x20, 0xB8);

        public string MyName => this.ReadValidString(new IntPtr(0x1405A2010), Config.PlayerNameSize);
        public int MySteamID32               => this.ReadInt32(new IntPtr(0x1405A2010));
        public int MyIndex
        {
            get
            {
                int players = LobbySize;
                if (players < 2) return 0;
                int steam = MySteamID32;
                for (int i = 0; i < players; i++)
                    if (steam == PlayerSteamID32(i))
                        return i;
                return 0;
            }
        }

        public int? MenuIndex
        {
            get
            {
                int menuBase = this.ReadInt32(new IntPtr(0x140573A78));
                if (menuBase == 0x0) return null;
                return this.ReadInt32(new IntPtr(menuBase + 0xA4 +
                this.ReadInt32(new IntPtr(menuBase + 0xE8)) * 0x04));
            }
        }

        public bool InInitial => (this.ReadByte(new IntPtr(0x1404640C2)) & 0b00100000) == 0b00100000;
        public bool InAdventure => this.ReadByte(new IntPtr(0x140451C50)) == 0b00000001 && (this.ReadByte(new IntPtr(0x140573854)) == 0);
        public bool InOnlineReplay => this.ReadByte(new IntPtr(0x1405989D0), 0x40, 0x28) != 0;
        public bool InLocalReplay => this.ReadByte(new IntPtr(0x140598BC8)) != 0;
        public bool InMatch => this.ReadInt32(new IntPtr(0x140461B20)) != 0;

        public bool PuzzleLeagueGameFinishFlag => this.ReadInt32(new IntPtr(0x140460690), 0xB4) == 1;

        public bool InReady
        {
            get
            {
                int pointer = this.ReadByte(new IntPtr(0x140460690), 0x280);
                return pointer != 0 && pointer != -1;
            }
        }

        public bool InCharacterSelection
        {
            get
            {
                int P1State = this.ReadByte(new IntPtr(0x140460690), 0x274);
                return (0 < P1State) && (P1State < 16);
            }
        }

        public int MainIDFromFlag
        {
            get
            {
                int online = this.ReadByte(new IntPtr(0x14059894C)) & 0b00000001;
                if (online > 0) return 4;

                int flags = this.ReadByte(new IntPtr(0x140451C50)) & 0b00010000;
                if (flags > 0) return 2;

                return 1;
            }
        }

        public int Mode(int id)
        {
            switch (id)
            {
                case 1:
                case 2:
                    return (this.ReadByte(new IntPtr(0x140451C50)) & 0b11101111) - 2;

                case 4:
                    return (this.ReadByte(new IntPtr(0x1404385C4)) > 0)
                        ? this.ReadByte(new IntPtr(0x140438584)) - 1
                        : this.ReadByte(new IntPtr(0x140573794));
            }
            return 0;
        }

        public int OnlineType => this.ReadByte(new IntPtr(0x140573797)) & 0b00000001;

        public int PlayerFinished(int index)
        {
            return this.ReadByte(new IntPtr(0x140575A04 + index));
        }


    }

    public static partial class ExtendedMethods
    {
        public static IntPtr ReadOffset(this PPTMemory pm, IntPtr pOffset, params Int32[] offsets)
        {
            IntPtr candidate = pOffset;
            foreach (Int32 tokenOffset in offsets)
            {
                try
                {
                    candidate = new IntPtr(pm.ReadInt32(candidate) + tokenOffset);
                }
                catch
                {
                    throw new InvalidOperationException(string.Format("ReadOffset for {0} broken, ({1})", pOffset, tokenOffset));
                }
            }
            return candidate;
        }

        public static bool ReadBinary(this PPTMemory pm, int location, IntPtr pOffset)
        {
            return pm.ReadBinary(location, pOffset, new int[] { });
        }

        public static bool ReadBinary(this PPTMemory pm, int location, IntPtr pOffset, params Int32[] offsets)
        {
            byte tempByte = pm.ReadByte(pm.ReadOffset(pOffset, offsets));

            return (tempByte & (1 << location)) != 0;
        }

        public static byte ReadByte(this PPTMemory pm, IntPtr pOffset, params Int32[] offsets)
        {
            return pm.ReadByte(pm.ReadOffset(pOffset, offsets));
        }

        public static Int32 ReadInt32(this PPTMemory pm, IntPtr pOffset, params Int32[] offsets)
        {
            return pm.ReadInt32(pm.ReadOffset(pOffset, offsets));
        }

        public static string ReadValidString(this PPTMemory pm, IntPtr pOffset, uint pSize, params Int32[] offsets)
        {
            string tempString = pm.ReadStringUnicode(pm.ReadOffset(pOffset, offsets), pSize);
            int nullIndex = tempString.IndexOf("\0");
            if (nullIndex > -1) tempString = tempString.Remove(nullIndex);

            return tempString;
        }
    }
}