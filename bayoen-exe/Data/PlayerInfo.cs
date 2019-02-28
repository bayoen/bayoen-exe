using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;

namespace bayoen.Data
{
    public class PlayerInfo : ICloneable
    {
        public int ID32 { get; private set; }
        public string Name { get; private set; }

        public int Rating { get; private set; }

        public string Region { get; private set; }
        public string Rank { get; private set; }
        public string League { get; private set; }

        //public int PlayStyle { get; private set; }

        public PPTPlayTypes PlayType { get; private set; }

        public PlayerInfo() { }

        public PlayerInfo(int index) => Check(index);

        public bool Check(int index)
        {
            this.ID32 = Core.PPTMemory.PlayerSteamID32Forced(index);
            this.Name = Core.PPTMemory.PlayerNameDirect(index);
            this.Rating = Core.PPTMemory.PlayerRating(index);
            this.PlayType = Core.PPTMemory.PlayType(index) ? PPTPlayTypes.Tetris : PPTPlayTypes.PuyoPuyo;

            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static PlayerInfo Load(string src)
        {
            PlayerInfo output = null;
            bool brokenFlag = false;
            if (File.Exists(src))
            {
                string rawString = File.ReadAllText(src, Config.TextEncoding);

                try
                {
                    output = js::JsonConvert.DeserializeObject<PlayerInfo>(rawString, Config.JSONSerializerSetting);
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
                output = new PlayerInfo();
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

        public static PlayerInfo FromJSON(jl::JObject jobject)
        {
            return js::JsonConvert.DeserializeObject<PlayerInfo>(jobject.ToString(), Config.JSONSerializerSetting);
        }
    }
}
