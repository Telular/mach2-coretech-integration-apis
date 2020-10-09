using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankUtilityTopicReader
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

        [JsonProperty("firmware_version")]
        public string FirmwareVersion;

        [JsonProperty("telemetry"), ]
        public List<Dictionary<string,object>> telemetry { get; set; }
    }
}
