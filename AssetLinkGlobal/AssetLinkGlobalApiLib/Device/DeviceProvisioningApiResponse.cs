using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetLinkGlobalApiLib.Device
{
    using AssetLinkGlobalApiLib.Device.Models;

    public class DeviceProvisioningApiResponse : BaseApiResponse
    {
        public List<DeviceProvisioningResult> Data { get; set; }
    }
}
