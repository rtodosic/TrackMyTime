using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace TrackMyTime
{
    [ExcludeFromCodeCoverageAttribute]
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .ConfigureAppConfiguration(configurationBuilder =>
                    {
                        configurationBuilder.AddJsonFile("azurekeys.json", true);
                    });
                });
    }
}
