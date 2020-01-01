using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TwitchSoft.Maintenance.Jobs;
using TwitchSoft.Shared.Database;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;

namespace TwitchSoft.Maintenance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<OldMessagesCleaner>()
                    .Hourly();

                scheduler
                    .Schedule<HoneymadFollowsJoin>()
                    .EveryFifteenMinutes();

                scheduler
                    .Schedule<EnsthorFollowsJoin>()
                    .EveryFifteenMinutes();
            });

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(loggerFactory =>
                {
                    //loggerFactory.ClearProviders();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Set up the objects we need to get to configuration settings
                    var Configuration = hostContext.Configuration;

                    services.AddScoped<IRepository, Repository>();
                    services.AddScoped<ITwitchApiService, TwitchApiService>();

                    services.AddDbContext<TwitchDbContext>(
                        options => options.UseSqlServer(Configuration.GetConnectionString(nameof(TwitchDbContext))));

                    services
                        .Configure<BotSettings>(Configuration.GetSection($"Twitch:{nameof(BotSettings)}"))
                        .AddOptions();

                    services.AddScheduler();

                    services.AddTransient<OldMessagesCleaner>();
                    services.AddTransient<HoneymadFollowsJoin>();
                    services.AddTransient<EnsthorFollowsJoin>();
                    services.AddTransient<TopChannelsJoin>();
                });
    }
}