using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device
{
    using AssetLinkGlobalApiLib.Device.Models;

    public class DeviceApiResponse : BaseApiResponse
    {
        public List<Device> Data { get; set; }
    }
}
