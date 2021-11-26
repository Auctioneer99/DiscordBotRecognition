using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace DiscordBotServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var port = Environment.GetEnvironmentVariable("PORT");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:" + port);
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) => 
                    {
                        config.AddEnvironmentVariables();
                        config.AddCommandLine(args);
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    });
                });
    }
}
