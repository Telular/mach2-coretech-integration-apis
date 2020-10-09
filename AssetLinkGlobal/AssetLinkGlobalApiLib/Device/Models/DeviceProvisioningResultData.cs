using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class DeviceProvisioningResultData
    {
        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "objects", NullValueHandling = NullValueHandling.Ignore)]
        public DeviceProvisioningResults Data { get; set; }
    }
}
