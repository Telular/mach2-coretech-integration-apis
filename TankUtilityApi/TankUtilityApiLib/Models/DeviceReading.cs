using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityApiLib.Models
{
    public class DeviceReading
    {
        [JsonProperty("tank")]
        public double Tank { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("time_iso")]
        public DateTime TimeIso { get; set; }

        [JsonProperty("telemetry", NullValueHandling = NullValueHandling.Ignore)]
        public List<Telemetry> Telemetry { get; set; }
    }
}
