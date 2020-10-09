using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class DeviceData
    {
        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "objects", NullValueHandling = NullValueHandling.Ignore)]
        public DeviceObjects Data { get; set; }
    }
}
