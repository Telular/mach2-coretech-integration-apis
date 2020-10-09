using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public partial class PointLocEstimate
    {
        [JsonProperty("Age", NullValueHandling = NullValueHandling.Ignore)]
        public string Age { get; set; }

        [JsonProperty("Radius", NullValueHandling = NullValueHandling.Ignore)]
        public string Radius { get; set; }

        [JsonProperty("LatEstimate", NullValueHandling = NullValueHandling.Ignore)]
        public string LatEstimate { get; set; }

        [JsonProperty("LonEstimate", NullValueHandling = NullValueHandling.Ignore)]
        public string LonEstimate { get; set; }
    }
}
