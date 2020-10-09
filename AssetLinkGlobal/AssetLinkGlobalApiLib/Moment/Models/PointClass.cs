using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    using AssetLinkGlobalApiLib.Moment.Converters;

    public class PointClass
    {
        [JsonProperty("SessionStatus", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SessionStatus { get; set; }

        [JsonProperty("MetaCEP", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? MetaCep { get; set; }

        [JsonProperty(PropertyName = "MetaLat", NullValueHandling = NullValueHandling.Ignore)]
        public string MetaLatitude { get; set; }

        [JsonProperty(PropertyName = "MetaLon", NullValueHandling = NullValueHandling.Ignore)]
        public string MetaLongitude { get; set; }

        [JsonProperty(PropertyName = "TimeSinceCom", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeSinceCom { get; set; }

        [JsonProperty(PropertyName = "AcqTime", NullValueHandling = NullValueHandling.Ignore)]
        public string AcqTime { get; set; }

        [JsonProperty("NumSats", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? NumSats { get; set; }

        [JsonProperty(PropertyName = "Battery", NullValueHandling = NullValueHandling.Ignore)]
        public string Battery { get; set; }

        [JsonProperty(PropertyName = "BatteryRaw", NullValueHandling = NullValueHandling.Ignore)]
        public string BatteryRaw { get; set; }

        [JsonProperty(PropertyName = "UnitTemperature", NullValueHandling = NullValueHandling.Ignore)]
        public string UnitTemperature { get; set; }

        [JsonProperty("CurrentMode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? CurrentMode { get; set; }

        [JsonProperty("Ack", NullValueHandling = NullValueHandling.Ignore)]
        public string Ack { get; set; }

        [JsonProperty("Retval", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Retval { get; set; }

        [JsonProperty("CmdsExecuted", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? CmdsExecuted { get; set; }

        [JsonProperty("Housekeeping", NullValueHandling = NullValueHandling.Ignore)]
        public string Housekeeping { get; set; }
    }
}
