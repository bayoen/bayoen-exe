using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bayoen.Data
{
    public class GameInfo : ICloneable
    {
        public List<PlayerInfo> Players;

        public int Frames
        {
            get
            {
                if (this.Players.Count < 2) return -1;
                return this.Players.First().Ticks.Last();
            }
        }

        public DateTime GameBegin { get; private set; }
        public DateTime GameEnd { get; private set; }

        public List<int> Winners { get; set; }

        public GameInfo()
        {
            
        }

        public bool Initialize()
        {
            this.GameBegin = DateTime.Now;
            this.GameEnd = DateTime.MinValue;
            this.Winners = new List<int>();
            return true;
        }

        public bool End()
        {
            this.GameEnd = DateTime.Now;
            return true;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
