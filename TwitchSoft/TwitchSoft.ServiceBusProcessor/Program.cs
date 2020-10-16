using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchSoft.ServiceBusProcessor;
using TwitchSoft.Shared;
using TwitchSoft.Shared.ElasticSearch;
using TwitchSoft.Shared.Logging;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.Services.TwitchApi;


CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureLogger()
        .ConfigureServices((hostContext, services) =>
        {
            services.ConfigureShared();
                    // Set up the objects we need to get to configuration settings
                    var Configuration = hostContext.Configuration;

            services.AddScoped<ITwitchApiService, TwitchApiService>();

            services.AddServiceBusProcessors(Configuration);

            services.AddCache(Configuration);
            services.AddElasticSearch(Configuration);
        });
