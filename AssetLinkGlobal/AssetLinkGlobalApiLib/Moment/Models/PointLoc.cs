using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public partial class PointLoc
    {
        [JsonProperty("Lat", NullValueHandling = NullValueHandling.Ignore)]
        public string Lat { get; set; }

        [JsonProperty("Lon", NullValueHandling = NullValueHandling.Ignore)]
        public string Lon { get; set; }
    }
}
