﻿using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.TwitchBot.ChatPlugins;
using TwitchSoft.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MediatR;
using TwitchLib.Client.Interfaces;
using Microsoft.Extensions.Options;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System;
using TwitchSoft.TwitchBot.Caching;

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
            services.AddCache(Configuration);

            services.ConfigureShared(Configuration);
            services.AddSingleton<TwitchBot>();

            services.AddHostedService<TwitchBotService>();
            services.AddHostedService<OrcherstratorClient>();

            services.AddServiceBusProcessors(Configuration);

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
