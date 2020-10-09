using System;
using System.Collections.Generic;


namespace AssetLinkGlobalApiLib
{
    using Device.Models;

    public class DeviceApiMultiRequestResponse : BaseApiResponse
    {
        public List<DeviceData> Data { get; set; }
    }
}
