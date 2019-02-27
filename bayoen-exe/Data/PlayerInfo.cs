using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bayoen.Memory;

namespace bayoen.Data
{
    public class PlayerInfo
    {
        public string ID3; // Steam ID
        public string Name; // Steam nickname
        public PPTGameModes GameMode;
        public List<int> Ticks; // Time ticks
        public List<int> Scores; // Scores ~ Ticks
    }
}
