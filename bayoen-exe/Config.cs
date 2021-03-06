﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using js = Newtonsoft.Json;
using bayoen.Utility.Json;
using System.Reflection;

namespace bayoen
{
    public class Config
    {
        public static readonly string PPTName = "puyopuyotetris";
        public static readonly AssemblyName ProjectAssemply = Assembly.GetExecutingAssembly().GetName();
        public static readonly TimeSpan PPTTimeSpan = new TimeSpan(0, 0, 0, 0, 5);

        public static readonly string StatFolderName = "stats";
        public static readonly uint PlayerNameSize = 36;

        public static readonly Encoding TextEncoding = Encoding.Unicode;

        public static readonly js::JsonSerializerSettings JSONSerializerSetting =  new js::JsonSerializerSettings()
        {
            NullValueHandling = js::NullValueHandling.Ignore,
            DateTimeZoneHandling = js::DateTimeZoneHandling.Local,
            Converters = new List<js::JsonConverter>()
            {
                new js::Converters.StringEnumConverter()
                {
                    NamingStrategy = new js::Serialization.DefaultNamingStrategy(),
                },
                new js::Converters.IsoDateTimeConverter()
                {
                    Culture = CultureInfo.CurrentCulture,
                    DateTimeStyles = DateTimeStyles.AssumeLocal,
                },
                new VersionConverter()
                {
                    
                },
            }
        };

        public static readonly int MinimumFrameTick = 20;

        public static readonly int MatchMax = 10;

        public static readonly double SplitterThickness = 4;

        public static readonly string[] ScorePlotColorHexes = { "#0099FF", "#FF6666", "#33CC33", "#FFCC33" };
    }
}
