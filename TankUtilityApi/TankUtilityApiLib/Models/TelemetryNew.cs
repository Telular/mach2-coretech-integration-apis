using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityApiLib.Models
{
    public class TelemetryNew
    {
        public Dictionary<string, object> Data { get; set; }

        [JsonProperty("nbc")]
        public List<Nbc> Nbc;

        [JsonProperty("fplmn")]
        public List<int> Fplmn;
    }
}
