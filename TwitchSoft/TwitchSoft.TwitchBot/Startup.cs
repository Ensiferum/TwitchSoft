using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.TwitchBot.ChatPlugins;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MediatR;
using TwitchLib.Client.Interfaces;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using AutoMapper;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System;

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
            services.ConfigureShared();
            services.AddSingleton<TwitchBot>();

            services
                .Configure<BotSettings>(Configuration.GetSection($"Twitch:{nameof(BotSettings)}"))
                .AddOptions();

            services.AddHostedService<TwitchBotService>();
            services.AddHostedService<OrcherstratorClient>();

            services.AddServiceBusProcessors(Configuration);

            services.AddCache(Configuration);

            services.AddTransient<IChatPlugin, KrippArenaBotChatPlugin>();
            services.AddTransient<IChatPlugin, RaffleParticipantBotChatPlugin>();

            services.AddSingleton<ITwitchClient>(sp =>
            {
                var botSettings = sp.GetService<IOptions<BotSettings>>();

                ConnectionCredentials credentials = new ConnectionCredentials(botSettings.Value.BotName, botSettings.Value.BotOAuthToken);

                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 10000,
                    ThrottlingPeriod = TimeSpan.FromSeconds(1)
                };
                var customClient = new WebSocketClient(clientOptions);
                var client =  new TwitchClient(customClient);
                client.Initialize(credentials);
                return client;
            });

            services.AddMediatR(typeof(Startup));

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
        }
    }
}
