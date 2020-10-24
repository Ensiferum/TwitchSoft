using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TwitchSoft.ServiceBusProcessor.Consumers;
using GreenPipes;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.ServiceBus.Configuration;

namespace TwitchSoft.ServiceBusProcessor
{
    public static class ConfigurationExtensions
    {
        public static void AddServiceBusProcessors(this IServiceCollection services, IConfiguration Configuration)
        {
            var serviceBusSettings = new ServiceBusSettings();
            Configuration.GetSection(nameof(ServiceBusSettings)).Bind(serviceBusSettings);
            ushort prefetchCount = 8;

            services.AddMassTransit(conf =>
            {
                conf.AddConsumer<NewTwitchChannelMessageConsumer>();
                conf.AddConsumer<NewSubscriberConsumer>();
                conf.AddConsumer<NewCommunitySubscriptionConsumer>();
                conf.AddConsumer<NewBanConsumer>();

                conf.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(serviceBusSettings.Host, serviceBusSettings.VirtualHost, hostConfigurator =>
                    {
                        hostConfigurator.Username(serviceBusSettings.Username);
                        hostConfigurator.Password(serviceBusSettings.Password);
                    });

                    cfg.ReceiveEndpoint("add-twitch-message", ep =>
                    {
                        ep.PrefetchCount = prefetchCount;
                        ep.UseRetry(r => r.Interval(5, 1000));

                        ep.ConfigureConsumer<NewTwitchChannelMessageConsumer>(context);
                        EndpointConvention.Map<NewTwitchChannelMessage>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint("add-twitch-subscriber", ep =>
                    {
                        ep.PrefetchCount = prefetchCount;
                        ep.UseMessageRetry(r => r.Interval(5, 1000));

                        ep.ConfigureConsumer<NewSubscriberConsumer>(context);
                        EndpointConvention.Map<NewSubscriber>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint("add-twitch-community-subscription", ep =>
                    {
                        ep.PrefetchCount = prefetchCount;
                        ep.UseMessageRetry(r => r.Interval(5, 1000));

                        ep.ConfigureConsumer<NewCommunitySubscriptionConsumer>(context);
                        EndpointConvention.Map<NewCommunitySubscription>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint("add-twitch-user-ban", ep =>
                    {
                        ep.PrefetchCount = prefetchCount;
                        ep.UseRetry(r => r.Interval(5, 1000));
                        ep.ConsumerPriority = -1;

                        ep.ConfigureConsumer<NewBanConsumer>(context);
                        EndpointConvention.Map<NewBan>(ep.InputAddress);
                    });
                }));
            });

            services.AddMassTransitHostedService();
        }
    }
}
