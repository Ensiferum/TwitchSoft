using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TwitchSoft.Shared.ServiceBus.Models;
using Microsoft.EntityFrameworkCore;
using TwitchSoft.Shared.Database;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.Models.Twitch;
using TwitchSoft.Shared.ServiceBus.Configuration;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.Helpers;
using TwitchSoft.Shared.Services.TwitchApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using TwitchSoft.TwitchBot.Grpc;
using TwitchSoft.TwitchBot.ChatPlugins;

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
                .ConfigureLogging(loggerFactory =>
                {
                    //loggerFactory.ClearProviders();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Set up the objects we need to get to configuration settings
                    var Configuration = hostContext.Configuration;

                    services.AddScoped<IRepository, Repository>();
                    services.AddScoped<ITwitchApiService, TwitchApiService>();
                    services.AddSingleton<TwitchBot>();

                    services
                        .Configure<BotSettings>(Configuration.GetSection($"Twitch:{nameof(BotSettings)}"))
                        .AddOptions();

                    services.AddDbContext<TwitchDbContext>(
                        options => options.UseSqlServer(Configuration.GetConnectionString(nameof(TwitchDbContext))));

                    services.AddHostedService<TwitchBotService>();
                    services.AddHostedService<TwitchBotGrpcServer>();

                    services.AddMassTransit(x =>
                    {
                        var serviceBusSettings = new ServiceBusSettings();
                        Configuration.GetSection(nameof(ServiceBusSettings)).Bind(serviceBusSettings);

                        x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var host = cfg.Host(serviceBusSettings.Host, serviceBusSettings.VirtualHost, hostConfigurator =>
                            {
                                hostConfigurator.Username(serviceBusSettings.Username);
                                hostConfigurator.Password(serviceBusSettings.Password);
                            });

                            cfg.ReceiveEndpoint("add-twitch-message", ep =>
                            {
                                EndpointConvention.Map<NewTwitchChannelMessage>(ep.InputAddress);
                            });

                            cfg.ReceiveEndpoint("add-twitch-subscriber", ep =>
                            {
                                EndpointConvention.Map<NewSubscriber>(ep.InputAddress);
                            });

                            cfg.ReceiveEndpoint("add-twitch-community-subscription", ep =>
                            {
                                EndpointConvention.Map<NewCommunitySubscription>(ep.InputAddress);
                            });

                            cfg.ReceiveEndpoint("add-twitch-user-ban", ep =>
                            {
                                EndpointConvention.Map<NewBan>(ep.InputAddress);
                            });
                        }));
                    });

                    services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
                    services.AddSingleton<IChannelsCache, ChannelsCache>();
                    services.AddTransient<TwitchBotGrpcService>();

                    services.AddTransient<IChatPlugin, KrippArenaBotChatPlugin>();
                });
    }
}
