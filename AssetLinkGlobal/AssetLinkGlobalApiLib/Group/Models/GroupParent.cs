using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Group.Models
{
    public class GroupParent
    {
        [JsonProperty(PropertyName = "grpid", Required = Required.Default)]
        public int GroupId { get; set; }
        [JsonProperty(PropertyName = "name", Required = Required.Default)]
        public string Name { get; set; }
    }
}
