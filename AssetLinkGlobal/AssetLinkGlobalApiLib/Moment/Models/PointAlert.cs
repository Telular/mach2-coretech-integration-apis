using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    using AssetLinkGlobalApiLib.Moment.Converters;

    public partial class PointAlert
    {
        [JsonProperty("Level", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Level { get; set; }

        [JsonProperty("ModeChange", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ModeChange { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("Panic")]
        public string Panic { get; set; }
    }
}
