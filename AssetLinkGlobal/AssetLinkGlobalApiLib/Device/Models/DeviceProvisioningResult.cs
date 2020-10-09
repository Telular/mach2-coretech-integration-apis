using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class DeviceProvisioningResult
    {
        [JsonProperty(PropertyName = "device", NullValueHandling = NullValueHandling.Ignore)]
        public Device Device { get; set; }
        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }
    }
}
