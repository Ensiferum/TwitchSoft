﻿using Coravel;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Maintenance.Jobs;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.Services.TwitchApi;
using TwitchSoft.Shared;
using Microsoft.Extensions.Logging;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using static TelegramBotGrpc;
using System;
using static TwitchBotGrpc;

namespace TwitchSoft.Maintenance
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

            services.AddGrpcClient<TelegramBotGrpcClient>(options =>
            {
                options.Address = new Uri($"https://{Configuration.GetValue<string>("Services:TelegramBot")}:80");
            });

            services.AddGrpcClient<TwitchBotGrpcClient>(options =>
            {
                options.Address = new Uri($"https://{Configuration.GetValue<string>("Services:TwitchBot")}:80");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.UseScheduler(scheduler =>
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
            }).LogScheduledTaskProgress(app.ApplicationServices.GetService<ILogger<IScheduler>>());
        }
    }
}