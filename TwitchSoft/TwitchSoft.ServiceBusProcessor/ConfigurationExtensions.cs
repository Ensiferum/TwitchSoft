using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TwitchSoft.ServiceBusProcessor.Consumers;
using GreenPipes;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.ServiceBus.Configuration;
using System;

namespace TwitchSoft.ServiceBusProcessor
{
    public static class ConfigurationExtensions
    {
        public static void AddServiceBusProcessors(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddMassTransit(x =>
            {
                var serviceBusSettings = new ServiceBusSettings();
                Configuration.GetSection(nameof(ServiceBusSettings)).Bind(serviceBusSettings);
                x.AddConsumer<NewTwitchChannelMessageConsumer>();
                x.AddConsumer<NewSubscriberConsumer>();
                x.AddConsumer<NewCommunitySubscriptionConsumer>();
                x.AddConsumer<NewBanConsumer>();

                ushort prefetchCount = 10;
                ushort batchPrefetchCount = 200;

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(serviceBusSettings.Host, serviceBusSettings.VirtualHost, hostConfigurator =>
                    {
                        hostConfigurator.Username(serviceBusSettings.Username);
                        hostConfigurator.Password(serviceBusSettings.Password);
                    });

                    //var logger = new LoggerConfiguration()
                    //    .ReadFrom.Configuration(Configuration)
                    //    .CreateLogger();

                    //cfg.UseSerilog(logger);

                    cfg.ReceiveEndpoint("add-twitch-message", ep =>
                    {
                        ep.PrefetchCount = batchPrefetchCount;
                        ep.UseRetry(r => r.Interval(5, 1000));

                        ep.Batch<NewTwitchChannelMessage>(b =>
                        {
                            b.MessageLimit = 100;

                            b.TimeLimit = TimeSpan.FromSeconds(1);

                            b.Consumer(() => provider.GetService<NewTwitchChannelMessageConsumer>());
                        });
                    });

                    cfg.ReceiveEndpoint("add-twitch-subscriber", ep =>
                    {
                        ep.PrefetchCount = prefetchCount;
                        ep.UseMessageRetry(r => r.Interval(5, 1000));

                        ep.ConfigureConsumer<NewSubscriberConsumer>(provider);
                        EndpointConvention.Map<NewSubscriber>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint("add-twitch-community-subscription", ep =>
                    {
                        ep.PrefetchCount = prefetchCount;
                        ep.UseMessageRetry(r => r.Interval(5, 1000));

                        ep.ConfigureConsumer<NewCommunitySubscriptionConsumer>(provider);
                        EndpointConvention.Map<NewCommunitySubscription>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint("add-twitch-user-ban", ep =>
                    {
                        ep.PrefetchCount = batchPrefetchCount;
                        ep.UseRetry(r => r.Interval(5, 1000));

                        ep.Batch<NewBan>(b =>
                        {
                            b.MessageLimit = 100;

                            b.TimeLimit = TimeSpan.FromSeconds(1);

                            b.Consumer(() => provider.GetService<NewBanConsumer>());
                        });
                    });
                }));
            });

            services.RegisterInMemorySagaRepository();

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            services.AddHostedService<BusService>();
        }
    }
}
