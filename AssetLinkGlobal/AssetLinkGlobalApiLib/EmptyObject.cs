using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace AssetLinkGlobalApiLib
{
    public class EmptyObject
    {
        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }
        [JsonProperty(PropertyName = "objects", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Objects { get; set; }
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public List<object> Data { get; set; }
    }
}
