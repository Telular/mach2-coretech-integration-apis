using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityApiLib.Models
{
    public class LastReadingNew
    {
        [JsonProperty("tank")]
        public double Tank;

        [JsonProperty("temperature")]
        public double Temperature;

        [JsonProperty("time")]
        public long Time;

        [JsonProperty("time_iso")]
        public DateTime TimeIso;
    }
}
