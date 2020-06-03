using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.TwitchBot.Grpc;
using TwitchSoft.TwitchBot.ChatPlugins;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace TwitchSoft.TwitchBot
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
            services.AddSingleton<TwitchBot>();

            services
                .Configure<BotSettings>(Configuration.GetSection($"Twitch:{nameof(BotSettings)}"))
                .AddOptions();

            services.AddHostedService<TwitchBotService>();

            services.AddServiceBusProcessors(Configuration);

            services.AddCache(Configuration);

            services.AddTransient<IChatPlugin, KrippArenaBotChatPlugin>();
            services.AddTransient<IChatPlugin, RaffleParticipantBotChatPlugin>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<TwitchBotGrpcService>();
            });
        }
    }
}
