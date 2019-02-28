using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using js = Newtonsoft.Json;

namespace bayoen
{
    public class Config
    {
        public static readonly string ProjectName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public static readonly string PPTName = "puyopuyotetris";

        public static readonly string StatFolderName = 

        public static readonly TimeSpan PPTTimeSpan = new TimeSpan(0, 0, 0, 0, 10);

        public static readonly uint PlayerNameSize = 36;

        public static readonly Encoding TextEncoding = Encoding.Unicode;

        public static readonly js::JsonSerializerSettings JSONSerializerSetting =  new js::JsonSerializerSettings()
        {
            NullValueHandling = js::NullValueHandling.Ignore,
            Converters = new List<js::JsonConverter>()
            {
                new js::Converters.StringEnumConverter
                {
                    NamingStrategy = new js::Serialization.DefaultNamingStrategy(),
                },
            }
        };

        public static readonly int MinimumFrameTick = 5;
}
}
