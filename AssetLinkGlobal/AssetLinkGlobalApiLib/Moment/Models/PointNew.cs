using Newtonsoft.Json;
using System.Collections.Generic;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    using static AssetLinkGlobal.MomentPointTypes;

    public partial class PointNew
    {
        public Dictionary<string, object> DataPoint { get; set; }

        [JsonProperty("Point", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointPoint { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_POINT); } }

        [JsonProperty("PointMsgType", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointMsgType { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_MSGTYPE); } }

        [JsonProperty("PointLoc", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointLoc { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_LOC); } }

        [JsonProperty("PointLocEstimate", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointLocEstimate { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_LOC_ESTIMATE); } }

        [JsonProperty("PointAlert", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointAlert { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_ALERT); } }

        [JsonProperty("PointGeofence", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointGeofence { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_GEOFENCE); } }

        [JsonProperty("PointPayload", NullValueHandling = NullValueHandling.Ignore)]
        private Dictionary<string, object> PointPayload { set { DataPoint = value; DataPoint.Add(POINT_TYPE_KEYNAME, POINT_PAYLOAD); } }
    }
}
