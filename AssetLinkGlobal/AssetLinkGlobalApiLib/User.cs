using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib
{
    public class User
    {
        [JsonProperty(PropertyName = "NAME")]
        public string Name;
        [JsonProperty(PropertyName = "PASSWORD")]
        public string Password;
    }
   
}
