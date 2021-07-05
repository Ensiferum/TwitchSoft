using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TwitchSoft.ServiceBusProcessor.Caching;
using TwitchSoft.Shared;
using static TelegramBotGrpc;

namespace TwitchSoft.ServiceBusProcessor
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
            services.AddCache();

            services.ConfigureShared(Configuration);

            services.AddServiceBusProcessors(Configuration);

            services.AddMediatR(typeof(Startup));

            services.AddGrpcClient<TelegramBotGrpcClient>(options =>
            {
                options.Address = new Uri(Configuration.GetValue<string>("Services:TelegramBot"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}