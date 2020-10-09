using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Moment.Models
{
    public class MomentData
    {

        [JsonProperty(PropertyName = "result", NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "error", NullValueHandling = NullValueHandling.Ignore)]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "objects", NullValueHandling = NullValueHandling.Ignore)]
        public MomentObjects Data { get; set; }
    }
}
