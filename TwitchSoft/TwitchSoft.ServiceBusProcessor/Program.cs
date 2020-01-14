using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchSoft.Shared;
using TwitchSoft.Shared.ElasticSearch;
using TwitchSoft.Shared.Logging;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.Services.TwitchApi;

namespace TwitchSoft.ServiceBusProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogger()
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureShared();
                    // Set up the objects we need to get to configuration settings
                    var Configuration = hostContext.Configuration;

                    services.AddScoped<ITwitchApiService, TwitchApiService>();

                    services.AddServiceBusProcessors(Configuration);

                    services.AddLocalRedisCache(Configuration);
                    services.AddElasticSearch(Configuration);
                });
    }
}
