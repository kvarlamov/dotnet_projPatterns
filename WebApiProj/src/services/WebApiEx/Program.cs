using System;
using System.IO;
using BaseTypes.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebApiEx
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //todo - change sdk to 6
            IHost host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(BaseConstants.AppSettingsFileName, false)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            Uri selfUri = new Uri(config.GetValue<string>(BaseConstants.SelfUrlConfigSectionName));
            
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(options => 
                            options.ListenAnyIP(selfUri.Port))
                        .UseStartup<Startup>();
                });
        }
    }
}