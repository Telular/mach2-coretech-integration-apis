using System.Configuration;

namespace TankUtilityApiLib.TankUtilityApi
{
    public class ApiOauthConfiguration : ConfigurationSection
    {        
        [ConfigurationProperty("authUrl", IsRequired = true)]
        public string AuthUrl => this["authUrl"] as string;

        //[ConfigurationProperty("tokenUrl", IsRequired = true)]
        //public string TokenUrl => this["tokenUrl"] as string;

        [ConfigurationProperty("authClientId", IsRequired = true)]
        public string AuthClientId => this["authClientId"] as string;

        [ConfigurationProperty("authClientSecret", IsRequired = true)]
        public string AuthClientSecret => this["authClientSecret"] as string;

        [ConfigurationProperty("apiUrl", IsRequired = true)]
        public string ApiUrl => this["apiUrl"] as string;

        [ConfigurationProperty("authRefreshToken", IsRequired = true)]
        public string AuthRefreshToken => this["authRefreshToken"] as string;
    }
}
