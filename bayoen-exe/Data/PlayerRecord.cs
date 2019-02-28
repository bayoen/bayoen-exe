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
    public class PlayerRecord : ICloneable
    {

        //public List<int> Scores { get; private set; }
        public List<int> Scores { get; set; }

        public PlayerRecord() { }

        public PlayerRecord(int index) => Initialize(index);

        public bool Initialize(int index)
        {
            this.Scores = new List<int>();

            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static PlayerRecord Load(string src)
        {
            PlayerRecord output = null;
            bool brokenFlag = false;
            if (File.Exists(src))
            {
                string rawString = File.ReadAllText(src, Config.TextEncoding);

                try
                {
                    output = js::JsonConvert.DeserializeObject<PlayerRecord>(rawString, Config.JSONSerializerSetting);
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
                output = new PlayerRecord();
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

        public static PlayerRecord FromJSON(jl::JObject jobject)
        {
            return js::JsonConvert.DeserializeObject<PlayerRecord>(jobject.ToString(), Config.JSONSerializerSetting);
        }
    }
}
