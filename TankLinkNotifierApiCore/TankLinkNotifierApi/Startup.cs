using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreRateLimit;


namespace TankLinkNotifierApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            

            services.AddSingleton<ServiceBusTopicSender>();
            services.AddSingleton<TankUtilityApi.TankUtilityApi>();
            

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer( opt =>
            {
                opt.Audience = Configuration["AAD:ResourceId"];
                opt.Authority = $"{Configuration["AAD:Instance"]}{Configuration["AAD:TenantId"]}";                
            });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);
                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.AddHttpContextAccessor();

            services.AddControllers().AddNewtonsoftJson();

            services.AddMemoryCache();

            services.AddStackExchangeRedisCache(options =>
            { 
                options.Configuration = Configuration["CacheConfigString"];
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
            }
            else                
            {
                app.UseExceptionHandler("/error");
            }

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();
           
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
