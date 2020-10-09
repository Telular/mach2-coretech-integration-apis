using System;
using System.Configuration;

namespace AssetLinkGlobalApiLib
{
   public class ApiConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("username", IsRequired = true)]
        public string Username => this["username"] as string;

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password => this["password"] as string;

        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public string BaseUrl => this["baseUrl"] as string;

        [ConfigurationProperty("authUrlPath", IsRequired = true)]
        public string AuthUrlPath => this["authUrlPath"] as string;

        [ConfigurationProperty("apiUrlPath", IsRequired = true)]
        public string ApiUrlPath => this["apiUrlPath"] as string;
    }
}
