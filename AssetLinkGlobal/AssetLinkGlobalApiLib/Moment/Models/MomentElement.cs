using System;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    using AssetLinkGlobalApiLib.Moment.Converters;

    public partial class MomentElement
    {
        [JsonProperty("momentid", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Momentid { get; set; }

        [JsonProperty("dateOriginated", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset DateOriginated { get; set; }

        [JsonProperty("dateReported", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset DateReported { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Type { get; set; }

        [JsonProperty("points", NullValueHandling = NullValueHandling.Ignore)]
        public PointNew[] Points { get; set; }

        [JsonProperty("dateReceived", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset DateReceived { get; set; }
    }
}
