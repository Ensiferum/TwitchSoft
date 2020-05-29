using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.Shared.Services.Models.Telegram;
using TwitchSoft.TelegramBot.Grpc;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.ElasticSearch;
using TwitchSoft.Shared.Logging;
using TwitchSoft.Shared;

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
                .ConfigureLogger()
                .ConfigureServices((hostContext, services) =>
                {
                    services.ConfigureShared();
                    // Set up the objects we need to get to configuration settings
                    var Configuration = hostContext.Configuration;

                    services.AddScoped<ITwitchApiService, TwitchApiService>();
                    services.AddSingleton<TelegramBot>();
                    services.AddTransient<TelegramBotGrpcService>();

                    services
                        .Configure<BotSettings>(Configuration.GetSection($"Telegram:{nameof(BotSettings)}"))
                        .Configure<Shared.Services.Models.Twitch.BotSettings>(Configuration.GetSection($"Twitch:{nameof(Shared.Services.Models.Twitch.BotSettings)}"))
                        .AddOptions();

                    services.AddHostedService<TelegramBotService>();
                    services.AddHostedService<TelegramBotGrpcServer>();

                    services.AddCache(Configuration);
                    services.AddElasticSearch(Configuration);
                });
    }
}
