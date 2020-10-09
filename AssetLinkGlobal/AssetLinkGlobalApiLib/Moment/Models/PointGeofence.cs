using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public partial class PointGeofence
    {

        [JsonProperty("fence", NullValueHandling = NullValueHandling.Ignore)]
        public string Fence { get; set; }

        [JsonProperty("set", NullValueHandling = NullValueHandling.Ignore)]
        public string Set { get; set; }

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [JsonProperty("Geofence", NullValueHandling = NullValueHandling.Ignore)]
        public string Geofence { get; set; }
    }

}
