using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib
{
    public class LoginUser : User
    {
        [JsonProperty(PropertyName = "STATUS")]
        public string Status;
    }

}
