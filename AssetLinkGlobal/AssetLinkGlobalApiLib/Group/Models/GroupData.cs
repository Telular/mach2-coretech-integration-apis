using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Group.Models
{
    public class GroupData
    {
        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public string Result { get; set; }

        [JsonProperty(PropertyName = "objects", Required = Required.Default)]
        public GroupObjects Data { get; set; }
    }
}
