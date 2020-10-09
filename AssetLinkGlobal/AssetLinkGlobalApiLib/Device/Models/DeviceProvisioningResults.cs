using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class DeviceProvisioningResults
    {
        [JsonProperty(PropertyName = "Devices", NullValueHandling = NullValueHandling.Ignore)]
        public List<DeviceProvisioningResult> DevicesResults { get; set; }
        [JsonProperty(PropertyName = "DevicesLimited", NullValueHandling = NullValueHandling.Ignore)]
        public int DevicesLimited { get; set; }
    }
}
