﻿using Coravel;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Maintenance.Jobs;
using TwitchSoft.Shared;
using Microsoft.Extensions.Logging;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using static TelegramBotGrpc;
using System;
using static TwitchBotOrchestratorGrpc;

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
            services.ConfigureShared(Configuration);

            services.AddScheduler();

            services.AddTransient<HoneymadFollowsJoin>();
            services.AddTransient<EnsthorFollowsJoin>();
            //services.AddTransient<SentDailyMessageDigest>();
            services.AddTransient<ChannelsRefresher>();
            services.AddTransient<OldMessagesCleaner>();
            services.AddTransient<ChannelsBanRefresher>();
            services.AddTransient<SubscriptionDailyCountCalculator>();

            services.AddGrpcClient<TelegramBotGrpcClient>(options =>
            {
                options.Address = new Uri(Configuration.GetValue<string>("Services:TelegramBot"));
            });

            services.AddGrpcClient<TwitchBotOrchestratorGrpcClient>(options =>
            {
                options.Address = new Uri(Configuration.GetValue<string>("Services:TwitchBotOrchestrator"));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.ApplicationServices.UseScheduler(scheduler =>
            {
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
                    .Schedule<OldMessagesCleaner>()
                    .Hourly();

                //scheduler
                //    .Schedule<SentDailyMessageDigest>()
                //    .Cron("0 7,13,19 * * *"); // every day at 7,13,19 utc 

                scheduler
                    .Schedule<ChannelsBanRefresher>()
                    .Cron("0 12 * * 1"); // every monday at 12 utc

                scheduler
                    .Schedule<SubscriptionDailyCountCalculator>()
                    .Cron("0 10 * * *"); // every day at 10 utc
            }).LogScheduledTaskProgress(app.ApplicationServices.GetService<ILogger<IScheduler>>());
        }
    }
}