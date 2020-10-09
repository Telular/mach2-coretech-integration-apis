using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public partial class Point
    {
        [JsonProperty("Point", NullValueHandling = NullValueHandling.Ignore)]
        public PointClass PointPoint { get; set; }

        [JsonProperty("PointMsgType", NullValueHandling = NullValueHandling.Ignore)]
        public PointMsgType PointMsgType { get; set; }

        [JsonProperty("PointLoc", NullValueHandling = NullValueHandling.Ignore)]
        public PointLoc PointLoc { get; set; }

        [JsonProperty("PointLocEstimate", NullValueHandling = NullValueHandling.Ignore)]
        public PointLocEstimate PointLocEstimate { get; set; }

        [JsonProperty("PointAlert", NullValueHandling = NullValueHandling.Ignore)]
        public PointAlert PointAlert { get; set; }

        [JsonProperty("PointGeofence", NullValueHandling = NullValueHandling.Ignore)]
        public PointGeofence PointGeofence { get; set; }

        [JsonProperty("PointPayload", NullValueHandling = NullValueHandling.Ignore)]
        public PointPayload PointPayload { get; set; }
    }
}
