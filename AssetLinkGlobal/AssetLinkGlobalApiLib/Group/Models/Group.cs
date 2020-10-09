using System.Collections.Generic;

using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Group.Models
{
    public class Group
    {
        [JsonProperty(PropertyName = "grpid", Required = Required.Always)]
        public int GroupId { get; set; }
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "type", Required = Required.AllowNull)]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "subtype", Required = Required.AllowNull)]
        public string Subtype { get; set; }
        [JsonProperty(PropertyName = "notes", Required = Required.AllowNull)]
        public string Notes { get; set; }
        [JsonProperty(PropertyName = "parent", Required = Required.AllowNull)]
        public List<GroupParent> Parents { get; set; }
        [JsonProperty(PropertyName = "child", Required = Required.AllowNull)]
        public List<object> Children { get; set; }
    }
}
