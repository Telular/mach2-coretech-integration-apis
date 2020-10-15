using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Log4Net;
//using Serilog;
//using Serilog.Events;
//using Serilog.Formatting.Compact;

namespace TankLinkNotifierApi
{
    public class Program
    {
        public static void Main(string[] args)
        {           

            try
            {                
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception anException)
            {
                Console.WriteLine($"Error in Main:  {anException.Message} {anException.InnerException?.Message}");

                return;
            }           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging((builder, logging) =>
                {                    
                    logging.ClearProviders();
                    logging.AddLog4Net("log4net.config");                    
                });
    }
}
