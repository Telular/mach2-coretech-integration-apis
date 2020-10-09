using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib
{
    public class Login
    {
        [JsonProperty(PropertyName = "USER")]
        public User User;
    }
}
