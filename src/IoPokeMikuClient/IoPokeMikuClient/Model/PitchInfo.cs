using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoPokeMikuClient.Model
{
    public class PitchInfo
    {
        public float f { get; set; }
        public int midi { get; set;}
    }

    public class RawData
    {
        public string id { get; set; }

        public UInt64 ts { get; set; }

        [JsonProperty("Params")]
        public PitchInfo pitch { get; set; }
    }
}
