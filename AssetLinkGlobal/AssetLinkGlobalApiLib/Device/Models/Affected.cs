using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class Affected
    {
        [JsonProperty(PropertyName = "file", Required = Required.Default)]
        public List<string> File { get; set; }
    }
}
