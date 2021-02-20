using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Models.Telegram;
using TwitchSoft.TelegramBot.Grpc;
using TwitchSoft.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using static TwitchBotOrchestratorGrpc;
using System;
using Telegram.Bot;
using Microsoft.Extensions.Options;
using MediatR;
using TwitchSoft.TelegramBot.TgCommands;

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

            services.ConfigureShared(Configuration);

            services.AddSingleton<TelegramBot>();
            services.AddSingleton<MessageProcessor>();

            services.AddHostedService<TelegramBotService>();

            services.AddSingleton<ITelegramBotClient>(sp =>
            {
                var botSettings = sp.GetService<IOptions<BotSettings>>();

                return new TelegramBotClient(botSettings.Value.BotOAuthToken);
            });

            services.AddTransient<BaseTgCommand, ListTopBySubscribersTgCommand>();
            services.AddTransient<BaseTgCommand, AddNewChannelTgCommand>();
            services.AddTransient<BaseTgCommand, GetSubscribersCountTgCommand>();
            services.AddTransient<BaseTgCommand, DailyNewSubscribersCountTgCommand>();
            services.AddTransient<BaseTgCommand, GetUserMessagesTgCommand>();
            services.AddTransient<BaseTgCommand, SearchTextTgCommand>();

            services.AddMediatR(typeof(Startup));

            services.AddAutoMapper(typeof(Startup));

            services.AddGrpcClient<TwitchBotOrchestratorGrpcClient>(o =>
            {
                o.Address = new Uri(Configuration.GetValue<string>("Services:TwitchBotOrchestrator"));
            });
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
