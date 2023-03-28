using System;
using System.IO;
using System.Runtime.Serialization;
using BaseTypes.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;

namespace WebApiEx
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
                })
                // в serilog большое количество sinks - пакетов, куда писать
                .UseSerilog((context, services, configuration) =>
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200")) //todo - to extension
                        {

                            FailureCallback = e =>
                            {
                                Console.WriteLine("Unable to submit event " + e.Exception);
                            },
                            FailureSink = new FileSink("./failures.txt", new JsonFormatter(), null),


                            TypeName = null,

                            IndexFormat = "webapiproj-log",
                            AutoRegisterTemplate = true,
                            EmitEventFailure = EmitEventFailureHandling.ThrowException | EmitEventFailureHandling.RaiseCallback | EmitEventFailureHandling.WriteToSelfLog
                        }));
        }
    }
}