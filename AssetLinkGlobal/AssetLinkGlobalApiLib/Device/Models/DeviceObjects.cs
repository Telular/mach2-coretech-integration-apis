
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class DeviceObjects
    {
        [JsonProperty(PropertyName = "Devices", Required = Required.Default)]
        public List<Device> Devices { get; set; }
        [JsonProperty(PropertyName = "DevicesLimited", Required = Required.Default)]
        public int DevicesLimited { get; set; }
    }
}
