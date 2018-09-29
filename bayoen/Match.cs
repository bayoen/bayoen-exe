using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using js = Newtonsoft.Json;
using jl = Newtonsoft.Json.Linq;

namespace bayoen
{
    public partial class MainWindow
    {
        public class Match
        {
            public DateTime? MatchStart;
            public DateTime? MatchEnd;

            public string Type;
            public string Mode;

            public List<int> Stars; // Stars.Count == 4 [#,#,#,#]
            public int? Winner;     // 
            public int? FirstToStar;    // 1,2,3 -?-> {Bo1, Bo3, Bo5}

            public List<Player> Players;

            public bool? IsBroken;
            public bool _isReplay;

            public const int playerMax = 4;

            public Match()
            {
                this.Clear();
            }

            public void Clear()
            {
                this.MatchStart = null;
                this.MatchEnd = null;

                this.Type = null;
                this.Mode = null;

                if (this.Stars == null) this.Stars = new List<int>(playerMax);
                else this.Stars.Clear();
                for (int starIndex = 0; starIndex < playerMax; starIndex++)
                {
                    this.Stars.Add(0);
                }

                if (this.Players == null)
                {
                    this.Players = new List<Player>(playerMax);
                }
                else
                {
                    this.Players.Clear();
                }
                for (int playerIndex = 0; playerIndex < playerMax; playerIndex++)
                {
                    this.Players.Add(new Player() { Position = playerIndex + 1 });
                }

                this.Winner = null;
                this.FirstToStar = null;

                this.IsBroken = null;
                this._isReplay = false;
            }

            public jl::JObject ToJSON()
            {
                jl::JObject output = new jl::JObject();

                if (this.MatchStart != null) output["MatchStart"] = this.MatchStart;
                if (this.MatchEnd != null) output["MatchEnd"] = this.MatchEnd;

                if (this.Type != null) output["Type"] = this.Type;
                if (this.Mode != null) output["Mode"] = this.Mode;

                if (this.Stars != null) output["Stars"] = jl::JArray.FromObject(this.Stars);
                if (this.Winner != null) output["Winner"] = this.Winner;
                if (this.FirstToStar != null) output["FirstToStar"] = this.FirstToStar;

                if (this.Players != null)
                {
                    jl::JArray tokenArray = new jl::JArray();
                    for (int listIndex = 0; listIndex < this.Players.Count; listIndex++)
                    {
                        tokenArray.Add(this.Players[listIndex].ToJSON());
                    }

                    output["Players"] = tokenArray;
                }

                if (this.IsBroken != null) output["IsBroken"] = this.IsBroken;

                return output;
            }

            public static Match FromJSON(jl::JObject jobject)
            {
                Match match = new Match();

                if (jobject.SelectToken("MatchStart") != null) match.MatchStart = DateTime.Parse(jobject["MatchStart"].ToString());
                if (jobject.SelectToken("MatchEnd") != null) match.MatchEnd = DateTime.Parse(jobject["MatchEnd"].ToString());

                if (jobject.SelectToken("Type") != null) match.Type = jobject["Type"].ToString();
                if (jobject.SelectToken("Mode") != null) match.Mode = jobject["Mode"].ToString();

                
                if (jobject.SelectToken("Stars") != null)
                {
                    if (jobject["Stars"].HasValues)
                    {
                        match.Stars = new List<int>();
                        foreach (jl::JToken token in jobject["Stars"].Children())
                        {
                            match.Stars.Add(int.Parse(token.ToString()));
                        }
                    }                    
                }   

                if (jobject.SelectToken("Winner") != null) match.Winner = int.Parse(jobject["Winner"].ToString());
                if (jobject.SelectToken("FirstToStar") != null) match.FirstToStar = int.Parse(jobject["FirstToStar"].ToString());

                if (jobject.SelectToken("Players") != null)
                {
                    match.Players = new List<Player>();
                    jl::JArray playerArray = jl::JArray.FromObject(jobject["Players"].ToArray());

                    foreach (jl::JObject objectToken in playerArray)
                    {
                        match.Players.Add(Player.FromJSON(objectToken));
                    }
                }

                match.IsBroken = false;
                match._isReplay = false;

                return match;
            }

        }
    }

}
