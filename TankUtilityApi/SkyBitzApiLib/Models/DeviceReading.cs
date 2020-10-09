using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SkyBitzApiLib.Models
{
    public class DeviceReading
    {
        [JsonProperty("tank")]
        public double Tank;

        [JsonProperty("temperature")]
        public double Temperature;

        [JsonProperty("time")]
        public long Time;

        [JsonProperty("time_iso")]
        public DateTime TimeIso;

        [JsonProperty("telemetry", NullValueHandling=NullValueHandling.Ignore)]
        public Dictionary<string, object> Telemetry;
    }
}
