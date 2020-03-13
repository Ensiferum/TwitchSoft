using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.Services.TwitchApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using TwitchSoft.TwitchBot.Grpc;
using TwitchSoft.TwitchBot.ChatPlugins;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.Logging;
using TwitchSoft.Shared;

namespace TwitchSoft.TwitchBot
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
                    services.AddSingleton<TwitchBot>();

                    services
                        .Configure<BotSettings>(Configuration.GetSection($"Twitch:{nameof(BotSettings)}"))
                        .AddOptions();

                    services.AddHostedService<TwitchBotService>();
                    services.AddHostedService<TwitchBotGrpcServer>();

                    services.AddServiceBusProcessors(Configuration);

                    services.AddCache(Configuration);
                    services.AddTransient<TwitchBotGrpcService>();

                    services.AddTransient<IChatPlugin, KrippArenaBotChatPlugin>();
                    services.AddTransient<IChatPlugin, RaffleParticipantBotChatPlugin>();
                });
    }
}
