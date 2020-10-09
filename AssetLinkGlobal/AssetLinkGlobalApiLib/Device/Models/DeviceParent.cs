using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class DeviceParent
    {
        [JsonProperty(PropertyName = "grpid", Required = Required.Default)]
        public int GroupId { get; set; }
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Name { get; set; }
    }
}
