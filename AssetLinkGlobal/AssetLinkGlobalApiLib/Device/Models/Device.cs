using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class Device
    {
        [JsonProperty(PropertyName = "deviceid", Required = Required.Default)]
        public int DeviceId { get; set; }
        [JsonProperty(PropertyName = "system", Required = Required.Default)]
        public string System { get; set; }
        [JsonProperty(PropertyName = "esn", Required = Required.Default)]
        public string ESN { get; set; }
        [JsonProperty(PropertyName = "lasttx", Required = Required.Default)]
        public int LastTx { get; set; }
        [JsonProperty(PropertyName = "lasttxtime", Required = Required.Default)]
        public DateTime LastTxTime { get; set; }
        [JsonProperty(PropertyName = "current", Required = Required.Default)]
        public Current Current { get; set; }
        [JsonProperty(PropertyName = "extras", Required = Required.Default)]
        public Extras Extras { get; set; }
        [JsonProperty(PropertyName = "affected", Required = Required.Default)]
        public Affected Affected { get; set; }
        [JsonProperty(PropertyName = "last_commanded_ids", Required = Required.Default)]
        public List<string> LastCommandIds { get; set; }
        [JsonProperty(PropertyName = "CLASS", Required = Required.Default)]
        public string Class { get; set; }
        [JsonProperty(PropertyName = "parent", Required = Required.Default)]
        public List<DeviceParent> Parents { get; set; }
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Name { get; set; }
    }
}
