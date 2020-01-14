using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchSoft.Maintenance.Jobs;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.Shared.Logging;
using TwitchSoft.Shared;

namespace TwitchSoft.Maintenance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler =>
            {
                //scheduler
                //    .Schedule<OldMessagesCleaner>()
                //    .Hourly();

                scheduler
                    .Schedule<HoneymadFollowsJoin>()
                    .EveryFifteenMinutes();

                scheduler
                    .Schedule<EnsthorFollowsJoin>()
                    .EveryFifteenMinutes();

                scheduler
                    .Schedule<ChannelsRefresher>()
                    .EveryFifteenMinutes();

                scheduler
                    .Schedule<SentDailyMessageDigest>()
                    .Cron("00 7,13,19 * * *");
            });

            host.Run();
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

                    services
                        .Configure<BotSettings>(Configuration.GetSection($"Twitch:{nameof(BotSettings)}"))
                        .AddOptions();

                    services.AddScheduler();

                    //services.AddTransient<OldMessagesCleaner>();
                    services.AddTransient<HoneymadFollowsJoin>();
                    services.AddTransient<EnsthorFollowsJoin>();
                    //services.AddTransient<TopChannelsJoin>();
                    services.AddTransient<SentDailyMessageDigest>();
                    services.AddTransient<ChannelsRefresher>();
                });
    }
}
