using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SkyBitzApiLib.SkyBitzApi
{
    public class SkyBitzApiResponse : ApiBaseResponse
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

    }
}
