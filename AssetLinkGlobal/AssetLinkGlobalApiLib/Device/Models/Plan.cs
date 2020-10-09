using System;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class Plan
    {
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "started", Required = Required.Default)]
        public DateTime Started { get; set; }
        [JsonProperty(PropertyName = "priorname", Required = Required.Default)]
        public object Priorname { get; set; }
        [JsonProperty(PropertyName = "priorstarted", Required = Required.Default)]
        public object Priorstarted { get; set; }
    }
}
