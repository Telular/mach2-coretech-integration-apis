using System.Collections.Generic;
using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Group.Models
{
    public class GroupObjects
    {
        [JsonProperty(PropertyName = "Grps", Required = Required.Default)]
        public List<Group> Groups { get; set; }
        [JsonProperty(PropertyName = "GrpsLimited", Required = Required.Default)]
        public int? GrpsLimited { get; set; }
    }
}
