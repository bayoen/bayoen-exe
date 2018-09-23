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
        public class Player
        {
            // Puzzle league infos.
            public string ID3;          // Player ID3 in steam
            public string Name;         // Player name
            public int? Rating;         // Puzzle league rating
            public string Rank;         // Puzzle league rank
            public string League;       // Puzzle league league
            public string Location;     // Regional location
            public int? Regional;       // Regional place
            public int? Worldwide;      // Worldwide place
            public int? PlayRatio;      // Puyo Puyo play ratio againt Tetris (10: All game puyo)

            // In game infos.
            public PlayTypes? PlayType;   // Puyopuyo? tetris?
            public int? Position;        // in game position
            //public Avatars? Avatar;      // in game avatar
            public List<bool> Stars;      // Star
            //public List<int> Scores;      // Score
            //public int? Delta;           // Increasement of rating

            public Player()
            {
                Clear();
            }

            public void Clear()
            {
                this.ID3 = null;
                this.Name = null;
                this.Rating = null;
                this.Rank = null;
                this.League = null;
                this.Location = null;
                this.Regional = null;
                this.Worldwide = null;
                this.PlayRatio = null;

                this.PlayType = null;
                //this.Position = null;


                if (this.Stars == null) this.Stars = new List<bool>();
                else this.Stars.Clear();

                //if (this.Scores == null) this.Scores = new List<int>();
                //else this.Scores.Clear();
            }

            public jl::JObject ToJSON()
            {
                return jl::JObject.Parse(js::JsonConvert.SerializeObject(this, new js::JsonSerializerSettings() { NullValueHandling = js::NullValueHandling.Ignore, }));
            }

            public static Player FromJSON(jl::JObject jobject)
            {
                return js::JsonConvert.DeserializeObject<Player>(jobject.ToString(), new js::JsonSerializerSettings() { NullValueHandling = js::NullValueHandling.Ignore, });
            }
        }
    }
    
}
