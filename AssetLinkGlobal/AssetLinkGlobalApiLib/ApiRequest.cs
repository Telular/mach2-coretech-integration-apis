using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib
{
   public class ApiRequest
    {
        [JsonProperty(PropertyName = "file", Required = Required.Always)]
        public string File;
        [JsonProperty(PropertyName = "action", Required = Required.Always)]
        public string Action;
        [JsonProperty(PropertyName = "filter", NullValueHandling = NullValueHandling.Ignore)]
        public string Filter;
        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data;
        [JsonProperty(PropertyName = "limit", NullValueHandling = NullValueHandling.Ignore)]
        public int? Limit;
        [JsonProperty(PropertyName = "offset", NullValueHandling = NullValueHandling.Ignore)]
        public int? Offset;
        [JsonProperty(PropertyName = "tag", NullValueHandling = NullValueHandling.Ignore)]
        public object Tag;
    }
}
