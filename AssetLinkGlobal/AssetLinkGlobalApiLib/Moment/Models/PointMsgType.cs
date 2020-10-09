using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    using AssetLinkGlobalApiLib.Moment.Converters;

    public partial class PointMsgType
    {
        [JsonProperty("num", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Num { get; set; }

        [JsonProperty("MsgType", NullValueHandling = NullValueHandling.Ignore)]
        public string MsgType { get; set; }

        [JsonProperty("extra", NullValueHandling = NullValueHandling.Ignore)]
        public string Extra { get; set; }
    }
}
