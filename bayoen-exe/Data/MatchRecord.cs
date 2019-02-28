using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bayoen.Memory;
using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;

namespace bayoen.Data
{
    public class MatchRecord : ICloneable
    {                 
        //public string MatchID { get; private set; }
        public PPTGameModes GameMode { get; private set; }
        public PPTGameCategories GameCategory { get; private set; }

        public DateTime MatchBegin { get; private set; }
        public DateTime MatchEnd { get; private set; }

        public int LobbyMax { get; private set; }
        public int LobbySize { get; private set; }
        public int WinCount { get; private set; }
        public List<int> Winners { get; private set; }

        private bool HeadReversed { get; set; }

        public List<PlayerInfo> Players { get; private set; }
        public List<GameRecord> Games { get; private set; }

        public bool Initialize()
        {
            // Already Checked!           
            this.GameMode = Core.PPTStatus.GameMode;
            this.GameCategory = this.MainStateToCatergory(Core.PPTStatus.MainState);
            this.MatchBegin = DateTime.UtcNow;
            this.MatchEnd = DateTime.MinValue;

            this.LobbyMax = Core.PPTStatus.LobbyMax;
            this.LobbySize = Core.PPTStatus.LobbySize;
            this.WinCount = Core.PPTMemory.WinCountForced;
            this.Winners = new List<int>();
            this.HeadReversed = this.IsHeaderRevered(Core.PPTStatus, Core.PPTMemory);

            this.Players = Enumerable.Range(0, Core.PPTStatus.LobbySize).Select(x => new PlayerInfo(x)).ToList();
            this.Games = new List<GameRecord>();

            return true;
        }

        public bool SaveCurrentGame()
        {
            this.Games.Add(Core.CurrentGame.Clone() as GameRecord);

            return true;
        }

        public List<int> MatchScores()
        {
            List<int> scores = new List<int>() { 0, 0, 0, 0 };
            this.Games.ForEach(x => x.Winners.ForEach(y => scores[y]++));
            return scores;
        }

        public void End()
        {
            this.MatchEnd = DateTime.UtcNow;

            List<int> scores = this.MatchScores();
            for (int playerIndex = 0; playerIndex < scores.Count; playerIndex++)
            {
                if (scores[playerIndex] == this.WinCount) this.Winners.Add(playerIndex);
            }            
        }

        private PPTGameCategories MainStateToCatergory(PPTMainStates state)
        {
            List<PPTGameCategories> categories = Enum.GetValues(typeof(PPTGameCategories)).Cast<PPTGameCategories>().ToList();
            int matchedIndex = categories.FindIndex(x => x.ToString() == state.ToString());
            if (matchedIndex < 0)
            {
                throw new InvalidOperationException($"Wrong PPTGameCategories cast from PPTMainStates.{state.ToString()}");
            }

            return categories[matchedIndex];
        }

        private bool IsHeaderRevered(PPTStatus status, PPTMemory memory)
        {
            if (status.LobbyMax > 2) return false;
            return (memory.MyIndex != 0);
        }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public static MatchRecord Load(string src)
        {
            MatchRecord output = null;
            bool brokenFlag = false;
            if (File.Exists(src))
            {
                string rawString = File.ReadAllText(src, Config.TextEncoding);

                try
                {
                    output = js::JsonConvert.DeserializeObject<MatchRecord>(rawString, Config.JSONSerializerSetting);
                }
                catch
                {
                    brokenFlag = true;
                }
            }
            else
            {
                brokenFlag = true;
            }

            if (brokenFlag)
            {
                output = new MatchRecord();
                File.WriteAllText(src, output.ToJSON().ToString(), Config.TextEncoding);
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
