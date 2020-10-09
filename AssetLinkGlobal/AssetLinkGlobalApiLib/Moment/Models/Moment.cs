using System;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    using AssetLinkGlobalApiLib.Moment.Converters;

    public partial class Moment
    {
        [JsonProperty("deviceid", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Deviceid { get; set; }

        [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
        public string System { get; set; }

        [JsonProperty("esn", NullValueHandling = NullValueHandling.Ignore)]
        public string Esn { get; set; }

        [JsonProperty("lasttxtime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset Lasttxtime { get; set; }

        [JsonProperty("moments", NullValueHandling = NullValueHandling.Ignore)]
        public MomentElement[] Moments { get; set; }

        [JsonProperty("CLASS", NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }
    }
}
