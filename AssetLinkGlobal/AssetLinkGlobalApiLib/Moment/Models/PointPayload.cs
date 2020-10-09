using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public class PointPayload
    {
        [JsonProperty("payload", NullValueHandling = NullValueHandling.Ignore)]
        public string payload { get; set; }
    }
}
