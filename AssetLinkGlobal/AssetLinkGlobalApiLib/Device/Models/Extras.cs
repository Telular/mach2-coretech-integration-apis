using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib.Device.Models
{
    public class Extras
    {
        [JsonProperty(PropertyName = "plan", Required = Required.Default)]
        public Plan Plan { get; set; }
        [JsonProperty(PropertyName = "configid", Required = Required.Default)]
        public string ConfigId { get; set; }
        [JsonProperty(PropertyName = "menu_variants", Required = Required.Default)]
        public string MenuVariants { get; set; }
    }
}
