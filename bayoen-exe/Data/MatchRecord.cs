﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bayoen.Data.Enums;
using bayoen.Memory;
using bayoen.Utility.ExternalMethods;

using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;

namespace bayoen.Data
{
    public class MatchRecord : ICloneable
    {
        //public string MatchID { get; private set; }
        [js::JsonProperty(PropertyName = "GameMode")]
        public GameModes GameMode { get; private set; }
        [js::JsonProperty(PropertyName = "GameCategory")]
        public GameCategories GameCategory { get; private set; }

        [js::JsonProperty(PropertyName = "MyName")]
        public string MyName { get; private set; }
        [js::JsonProperty(PropertyName = "MyID32")]
        public int MyID32 { get; private set; }

        [js::JsonProperty(PropertyName = "MatchBegin")]
        public DateTime MatchBegin { get; private set; }
        [js::JsonProperty(PropertyName = "MatchEnd")]
        public DateTime MatchEnd { get; private set; }

        [js::JsonProperty(PropertyName = "MatchCrash")]
        public MatchCrashes MatchCrash { get; private set; }

        [js::JsonProperty(PropertyName = "LobbyMax")]
        public int LobbyMax { get; private set; }
        [js::JsonProperty(PropertyName = "LobbySize")]
        public int LobbySize { get; private set; }
        [js::JsonProperty(PropertyName = "WinCount")]
        public int WinCount { get; private set; }
        [js::JsonProperty(PropertyName = "Wins")]
        public List<int> Wins { get; private set; }
        [js::JsonProperty(PropertyName = "Places")]
        public List<int> Places { get; private set; }
        [js::JsonProperty(PropertyName = "Winners")]
        public List<int> Winners { get; private set; }
        [js::JsonProperty(PropertyName = "RatingGain")]
        public int RatingGain { get; private set; }

        private bool _isHeadReversed { get; set; }

        [js::JsonProperty(PropertyName = "Players")]
        public List<PlayerInfo> Players { get; private set; }
        [js::JsonProperty(PropertyName = "Games")]
        public List<GameRecord> Games { get; private set; }

        [js::JsonProperty(PropertyName = "DataVersion")]
        public static Version DataVersion = new Version(0, 1, 1);

        public bool Reset()
        {
            this.MatchBegin = DateTime.MinValue;
            this.MatchEnd = DateTime.MinValue;

            return true;
        }

        public bool Initialize()
        {
            // Already Checked!           
            this.GameMode = Core.PPTStatus.GameMode;
            this.GameCategory = this.MainStateToCatergory(Core.PPTStatus.MainState);
            this.MyName = Core.PPTMemory.MyName;
            this.MyID32 = Core.PPTMemory.MyID32;
                       
            this.MatchBegin = DateTime.UtcNow;
            this.MatchEnd = DateTime.MinValue;
            this.MatchCrash = MatchCrashes.None;

            this.LobbyMax = Core.PPTStatus.LobbyMax;
            this.LobbySize = Core.PPTStatus.LobbySize;
            this.WinCount = Core.PPTMemory.WinCountForced;
            this.Wins = Enumerable.Repeat(0, this.LobbySize).ToList();
            this.Winners = new List<int>();
            this.RatingGain = 0;
            this._isHeadReversed = this.IsHeaderRevered(Core.PPTStatus, Core.PPTMemory);

            this.Players = Enumerable.Range(0, Core.PPTStatus.LobbySize).Select(x => new PlayerInfo(x)).ToList();
            this.Games = new List<GameRecord>();

            return true;
        }

        public bool GetRatingGain()
        {
            this.RatingGain = Core.PPTStatus.MyRating - Core.OldPPTStatus.MyRating;

            return true;
        }

        public bool SaveCurrentGame()
        {
            this.Games.Add(Core.CurrentGame.Clone() as GameRecord);

            return true;
        }

        public List<int> MatchWins()
        {
            List<int> scores = Enumerable.Repeat(0, this.LobbySize).ToList();
            this.Games.ForEach(x => x.Winners.ForEach(y => scores[y-1]++));
            return scores;
        }

        public void End()
        {
            this.MatchEnd = DateTime.UtcNow;

            this.Wins = this.MatchWins();
            for (int playerIndex = 0; playerIndex < this.Wins.Count; playerIndex++)
            {                
                if (this.Wins[playerIndex] == this.WinCount) this.Winners.Add(playerIndex+1);
            }            
        }

        public void Crashed()
        {
            this.MatchCrash = MatchCrashes.NotMe;
        }

        private GameCategories MainStateToCatergory(MainStates state)
        {
            List<GameCategories> categories = Enum.GetValues(typeof(GameCategories)).Cast<GameCategories>().ToList();
            int matchedIndex = categories.FindIndex(x => x.ToString() == state.ToString());
            if (matchedIndex < 0)
            {
                throw new InvalidOperationException($"Wrong PPTGameCategories cast from MainStates.{state.ToString()}");
            }

            return categories[matchedIndex];
        }

        private bool IsHeaderRevered(PPTStatus status, PPTMemory memory)
        {
            if (status.LobbyMax > 2) return false;
            return (memory.MyIndex != 0);
        }

        [js.JsonIgnore]
        public string TimeColumn
        {
            get
            {
                //if (this.MatchEnd.ToLocalTime().Date == DateTime.Today)
                //{
                //    return this.MatchEnd.ToLocalTime().ToString("tt hh:mm:ss", CultureInfo.CreateSpecificCulture("en-US"));
                //}
                //else
                //{
                //    return this.MatchEnd.ToLocalTime().ToString("d MMM yy", CultureInfo.CreateSpecificCulture("en-US"));
                //}
                return this.MatchEnd.ToLocalTime().ToSimpleString();
            }
        }

        [js.JsonIgnore]
        public string MyPlayTypeColumn
        {
            get
            {
                PlayerInfo myInfo = this.Players.Find(x => x.ID32 == this.MyID32);
                if (myInfo == null) return "Not me";

                return myInfo.PlayType.ToString();
            }
        }

        [js.JsonIgnore]
        public string OpponentInfoColumn
        {
            get
            {
                PlayerInfo opponentInfo = this.Players.Find(x => x.ID32 != this.MyID32);
                if (opponentInfo == null) return "No opponent";

                return $"{opponentInfo.Name} ({opponentInfo.Rating}, {opponentInfo.PlayType.ToString()})";
            }
        }

        [js.JsonIgnore]
        public string ResultColumn
        {
            get
            {
                if (this.MatchCrash != MatchCrashes.None) return "Crashed";

                string gameResults = "";
                int myLocation = 1 + this.Players.FindIndex(x => x.ID32 == this.MyID32); // 1 or 2
                foreach (GameRecord game in this.Games)
                {
                    if (game.Winners.Count == 0)
                    {
                        gameResults += "D";
                    }
                    else if (game.Winners.Contains(myLocation))
                    {
                        gameResults += "W";
                    }
                    else // if (!game.Winners.Contains(myLocation))
                    {
                        gameResults += "L";
                    }
                }
                if (this.Games.Count > 0) gameResults = $" ({gameResults})";

                return $"{this.RatingGain.ToString("+#;-#;0")}{gameResults}";
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static MatchRecord Load(string src)
        {
            MatchRecord output = null;
            if (File.Exists(src))
            {
                string rawString = File.ReadAllText(src, Config.TextEncoding);

                try
                {
                    output = js::JsonConvert.DeserializeObject<MatchRecord>(rawString);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
            return output;
        }

        public bool Save(string dst)
        {
            try
            {
                File.WriteAllText(dst, this.ToJSON().ToString(), Config.TextEncoding);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public jl::JObject ToJSON()
        {
            return jl::JObject.Parse(js::JsonConvert.SerializeObject(this, Config.JSONSerializerSetting));
        }

        public static MatchRecord FromJSON(jl::JObject jobject)
        {
            return js::JsonConvert.DeserializeObject<MatchRecord>(jobject.ToString(), Config.JSONSerializerSetting);
        }
    }
}
