using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TankLinkNotifierApi.TankUtilityApi
{
    public class TankUtilityApiResponse : ApiBaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

    }
}
