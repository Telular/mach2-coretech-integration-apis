using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    using Moment.Converters;

    public class OutgoingMBX
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("momid")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Momid { get; set; }
    }
}
