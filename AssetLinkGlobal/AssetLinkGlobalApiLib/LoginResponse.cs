using Newtonsoft.Json;

namespace AssetLinkGlobalApiLib
{   
    public class LoginResponse : BaseApiResponse
    {
        public LoginResponse()
        {
            User = new LoginUser();
        }

        [JsonProperty(PropertyName = "USER")]
        public LoginUser User { get; private set; }
    }
}
