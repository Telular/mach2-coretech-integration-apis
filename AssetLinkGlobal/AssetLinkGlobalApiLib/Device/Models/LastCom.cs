using System;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
   public class LastCom
    {
        [JsonProperty(PropertyName = "ALL", Required = Required.Default)]
        public DateTime ALL { get; set; }
        [JsonProperty(PropertyName = "ISB", Required = Required.Default)]
        public DateTime ISB { get; set; }
        [JsonProperty(PropertyName = "VIA", Required = Required.Default)]
        public string VIA { get; set; }
    }
}
