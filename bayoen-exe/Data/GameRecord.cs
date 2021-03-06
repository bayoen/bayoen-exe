﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;

namespace bayoen.Data
{
    public class GameRecord : ICloneable
    {
        [js::JsonProperty(PropertyName = "GameBegin")]
        public DateTime GameBegin { get; private set; }
        [js::JsonProperty(PropertyName = "GameEnd")]
        public DateTime GameEnd { get; private set; }
        [js::JsonProperty(PropertyName = "Winners")]
        public List<int> Winners { get; private set; }
        [js::JsonProperty(PropertyName = "Places")]
        public List<int> Places { get; private set; }
        [js::JsonProperty(PropertyName = "Ticks")]
        public List<int> Ticks { get; private set; }
        [js::JsonProperty(PropertyName = "PlayerRecords")]
        public List<PlayerRecord> PlayerRecords { get; private set; }
        [js::JsonIgnore]
        private int LastFrameTick { get; set; }

        public bool Initialize()
        {
            this.PlayerRecords = Enumerable.Range(0, Core.PPTStatus.LobbySize).Select(x => new PlayerRecord(x)).ToList();
            this.GameBegin = DateTime.UtcNow;
            this.GameEnd = DateTime.MinValue;
            this.Ticks = new List<int>();
            this.Winners = new List<int>();
            this.LastFrameTick = -Config.MinimumFrameTick - 1;

            return true;
        }

        public void CheckTickScores()
        {
            if (this.GameBegin != DateTime.MinValue && this.GameEnd == DateTime.MinValue)
            {
                if (Core.PPTStatus.GameFrame - this.LastFrameTick > Config.MinimumFrameTick)
                {
                    Core.CurrentGame.Ticks.Add(Core.PPTStatus.GameFrame);
                    for (int playerIndex = 0; playerIndex < this.PlayerRecords.Count; playerIndex++)
                    {
                        PlayerRecords[playerIndex].Scores.Add(Core.PPTMemory.PlayerScore(playerIndex));
                    }

                    this.LastFrameTick = Core.PPTStatus.GameFrame;
                }
            }
        }

        public bool End()
        {
            this.GameEnd = DateTime.UtcNow;
            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static GameRecord Load(string src)
        {
            GameRecord output = null;
            bool brokenFlag = false;
            if (File.Exists(src))
            {
                string rawString = File.ReadAllText(src, Config.TextEncoding);

                try
                {
                    output = js::JsonConvert.DeserializeObject<GameRecord>(rawString, Config.JSONSerializerSetting);
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
                output = new GameRecord();
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

        public static GameRecord FromJSON(jl::JObject jobject)
        {
            return js::JsonConvert.DeserializeObject<GameRecord>(jobject.ToString(), Config.JSONSerializerSetting);
        }        
    }
}
