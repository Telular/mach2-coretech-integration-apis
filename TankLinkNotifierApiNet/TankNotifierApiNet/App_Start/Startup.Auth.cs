using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Owin;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.ActiveDirectory;

namespace TankNotifierApiNet.App_Start
{
    
    public partial class Startup
    {        

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
                        
            string instanceId = ConfigurationManager.AppSettings["InstanceId"];
            string tenantId = ConfigurationManager.AppSettings["TenantId"];
            string issuer = $"{instanceId}{tenantId}";
            string audience = ConfigurationManager.AppSettings["ResourceId"];

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = tenantId,
                    //Set SaveSigninToken to true, to be able to retrieve the bearer token using the code: ClaimsPrincipal.Current.Identities.First().BootstrapContext
                    //This flag is mandatory for this flow to work
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        SaveSigninToken = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = false,
                        ValidIssuer = issuer,
                        ValidAudience = audience                        
                    }
                });


        }
    }
}