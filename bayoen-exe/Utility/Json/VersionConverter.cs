using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace bayoen.Utility.Json
{
    public class VersionConverter : JsonConverter<Version>
    {
        public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = (string)reader.Value;

            return new Version(s);
        }
    }
}
