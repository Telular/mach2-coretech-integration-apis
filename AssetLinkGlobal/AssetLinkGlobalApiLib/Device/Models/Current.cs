
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class Current
    {
        [JsonProperty(PropertyName = "Lat", Required = Required.Default)]
        public double Lat { get; set; }
        [JsonProperty(PropertyName = "Lon", Required = Required.Default)]
        public double Lon { get; set; }
        [JsonProperty(PropertyName = "mode", Required = Required.Default)]
        public int Mode { get; set; }
        [JsonProperty(PropertyName = "cmdtag", Required = Required.Default)]
        public string CommandTag { get; set; }
        [JsonProperty(PropertyName = "script", Required = Required.Default)]
        public object Script { get; set; }
        [JsonProperty(PropertyName = "LastCom", Required = Required.Default)]
        public LastCom LastCommunication { get; set; }
        [JsonProperty(PropertyName = "configc", Required = Required.Default)]
        public int ConfigC { get; set; }
        [JsonProperty(PropertyName = "OutgoingMbx", Required = Required.Default)]
        public List<OutgoingMBX> OutgoingMBX { get; set; }
    }
}
