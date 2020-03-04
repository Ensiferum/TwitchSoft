using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TwitchSoft.ServiceBusProcessor.Consumers;
using GreenPipes;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.ServiceBus.Configuration;
using MassTransit.Context;

namespace TwitchSoft.ServiceBusProcessor
{
    public static class ConfigurationExtensions
    {
        public static void AddServiceBusProcessors(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<NewTwitchChannelMessageConsumer>();
                x.AddConsumer<NewSubscriberConsumer>();
                x.AddConsumer<NewCommunitySubscriptionConsumer>();
                x.AddConsumer<NewBanConsumer>();
            });

            LogContext.ConfigureCurrentLogContext();

            var serviceBusSettings = new ServiceBusSettings();
            Configuration.GetSection(nameof(ServiceBusSettings)).Bind(serviceBusSettings);
            ushort prefetchCount = 10;

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(serviceBusSettings.Host, serviceBusSettings.VirtualHost, hostConfigurator =>
                {
                    hostConfigurator.Username(serviceBusSettings.Username);
                    hostConfigurator.Password(serviceBusSettings.Password);
                });

                cfg.ReceiveEndpoint("add-twitch-message", ep =>
                {
                    ep.PrefetchCount = prefetchCount;
                    ep.UseRetry(r => r.Interval(5, 1000));

                    ep.ConfigureConsumer<NewTwitchChannelMessageConsumer>(provider);
                    EndpointConvention.Map<NewTwitchChannelMessage>(ep.InputAddress);
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
                    ep.PrefetchCount = prefetchCount;
                    ep.UseRetry(r => r.Interval(5, 1000));

                    ep.ConfigureConsumer<NewBanConsumer>(provider);
                    EndpointConvention.Map<NewBan>(ep.InputAddress);
                });
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            services.AddHostedService<BusService>();
        }
    }
}
