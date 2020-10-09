using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankLinkNotifierApi.Models
{
    public class TelemetryNew
    {
        public Dictionary<string, object> Data { get; set; }

        [JsonProperty("nbc")]
        public List<NbcNew> Nbc;

        [JsonProperty("fplmn")]
        public List<int> Fplmn;
    }
}
