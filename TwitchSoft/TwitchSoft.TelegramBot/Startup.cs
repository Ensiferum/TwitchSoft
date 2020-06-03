using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.Shared.Services.Models.Telegram;
using TwitchSoft.TelegramBot.Grpc;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.ElasticSearch;
using TwitchSoft.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

namespace TwitchSoft.TelegramBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.ConfigureShared();

            services.AddScoped<ITwitchApiService, TwitchApiService>();
            services.AddSingleton<TelegramBot>();

            services
                .Configure<BotSettings>(Configuration.GetSection($"Telegram:{nameof(BotSettings)}"))
                .Configure<Shared.Services.Models.Twitch.BotSettings>(Configuration.GetSection($"Twitch:{nameof(Shared.Services.Models.Twitch.BotSettings)}"))
                .AddOptions();

            services.AddHostedService<TelegramBotService>();

            services.AddCache(Configuration);
            services.AddElasticSearch(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TelegramBotGrpcService>();
            });
        }
    }
}
