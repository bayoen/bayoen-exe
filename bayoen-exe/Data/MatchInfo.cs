using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bayoen.Memory;
using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;
using System.IO;

namespace bayoen.Data
{
    public class MatchInfo
    {                 
        public List<GameInfo> Games { get; private set; }

        public int WinCount { get; private set; }

        public PPTGameModes GameMode { get; private set; }
        public PPTGameCategories GameCategory { get; private set; }

        public DateTime MatchBegin { get; private set; }
        public DateTime MatchEnd { get; private set; }

        public string MatchID { get; private set; }

        public int LobbySize { get; private set; }

        public int LobbyMax { get; private set; }

        public bool Initialize()
        {
            // Already Checked!
            this.Games = new List<GameInfo>();
            this.WinCount = Core.PPTMemory.WinCountForced;
            this.GameMode = Core.PPTStatus.GameMode;
            this.GameCategory = this.MainStateToCatergory(Core.PPTStatus.MainState);
            this.MatchBegin = DateTime.Now;
            this.MatchEnd = DateTime.MinValue;

            this.LobbySize = Core.PPTStatus.LobbySize;
            this.LobbyMax = Core.PPTStatus.LobbyMax;

            return true;
        }

        public bool SaveCurrentGame()
        {
            this.Games.Add(Core.CurrentGame.Clone() as GameInfo);

            return true;
        }

        public void End()
        {
            this.MatchEnd = DateTime.Now;
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

        public static MatchInfo Load(string src)
        {
            MatchInfo output = null;
            bool brokenFlag = false;
            if (File.Exists(src))
            {
                string rawString = File.ReadAllText(src, Config.TextEncoding);

                try
                {
                    output = js::JsonConvert.DeserializeObject<MatchInfo>(rawString, Config.JSONSerializerSetting);
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
                output = new MatchInfo();
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

        public static MatchInfo FromJSON(jl::JObject jobject)
        {
            return js::JsonConvert.DeserializeObject<MatchInfo>(jobject.ToString(), Config.JSONSerializerSetting);
        }
    }
}
