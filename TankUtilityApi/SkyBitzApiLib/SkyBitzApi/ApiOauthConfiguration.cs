using System.Configuration;


namespace SkyBitzApiLib.SkyBitzApi
{ 

    public class ApiOauthConfiguration : ConfigurationSection
    {        
        [ConfigurationProperty("authUrl", IsRequired = true)]
        public string AuthUrl => this["authUrl"] as string;                

        [ConfigurationProperty("authClientId", IsRequired = true)]
        public string AuthClientId => this["authClientId"] as string;

        [ConfigurationProperty("authClientSecret", IsRequired = true)]
        public string AuthClientSecret => this["authClientSecret"] as string;

        [ConfigurationProperty("apiUrl", IsRequired = true)]
        public string ApiUrl => this["apiUrl"] as string;

        [ConfigurationProperty("authTenantId", IsRequired = true)]
        public string AuthTenantId => this["authTenantId"] as string;

        [ConfigurationProperty("authScopeAppId", IsRequired = true)]
        public string AuthScopeId => this["authScopeAppId"] as string;
    }
}
