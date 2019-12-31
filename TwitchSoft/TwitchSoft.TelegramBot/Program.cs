using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.Shared.Services.Models.Telegram;
using TwitchSoft.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TwitchSoft.TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                    services.AddSingleton<TelegramBot>();

                    services
                        .Configure<BotSettings>(Configuration.GetSection($"Telegram:{nameof(BotSettings)}"))
                        .Configure<Shared.Services.Models.Twitch.BotSettings>(Configuration.GetSection($"Twitch:{nameof(Shared.Services.Models.Twitch.BotSettings)}"))
                        .AddOptions();

                    services.AddDbContext<TwitchDbContext>(
                        options => options.UseSqlServer(Configuration.GetConnectionString(nameof(TwitchDbContext))));

                    services.AddHostedService<TelegramBotService>();
                });
    }
}
