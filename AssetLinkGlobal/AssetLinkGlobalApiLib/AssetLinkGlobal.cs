using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetLinkGlobalApiLib
{
    public static class AssetLinkGlobal
    {
        public static class ApiFileIdentifiers
        {
            public const string DEVICE = "device";
            public const string MOMMENT = "moment";
            public const string GROUP = "grp";
            public const string REMOTE = "remote";
        }

        public static class ApiActionsIdentifiers
        {
            public const string GET = "get";
            public const string PROVISION = "provision";
            public const string DEPROVISION = "deprovision";
            public const string REPORT = "report";
            public const string SPIFLASH = "spiflash";
        }

      
        public static class MomentPointTypes
        {
            public const string POINT_TYPE_KEYNAME = "DataPointType";
            public const string POINT_POINT = "Point";
            public const string POINT_MSGTYPE = "PointMsgType";
            public const string POINT_LOC = "PointLoc";
            public const string POINT_LOC_ESTIMATE = "PointLocEstimate";
            public const string POINT_ALERT = "PointAlert";
            public const string POINT_GEOFENCE = "PointGeofence";
            public const string POINT_PAYLOAD = "PointPayload";
            public const string POINT_SENSOR = "PointSensor";
            public const string POINT_SEQUENCE = "PointSequence";
        }

    }
}
