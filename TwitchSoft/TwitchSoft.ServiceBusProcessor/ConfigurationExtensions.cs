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
            services.AddMassTransit(x =>
            {
                var serviceBusSettings = new ServiceBusSettings();
                Configuration.GetSection(nameof(ServiceBusSettings)).Bind(serviceBusSettings);
                x.AddConsumer<NewTwitchChannelMessageConsumer>();
                x.AddConsumer<NewSubscriberConsumer>();
                x.AddConsumer<NewCommunitySubscriptionConsumer>();
                x.AddConsumer<NewBanConsumer>();

                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(serviceBusSettings.Host, serviceBusSettings.VirtualHost, hostConfigurator =>
                    {
                        hostConfigurator.Username(serviceBusSettings.Username);
                        hostConfigurator.Password(serviceBusSettings.Password);
                    });

                    cfg.ReceiveEndpoint(host, "add-twitch-message", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));

                        ep.ConfigureConsumer<NewTwitchChannelMessageConsumer>(provider);
                        EndpointConvention.Map<NewTwitchChannelMessage>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint(host, "add-twitch-subscriber", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));

                        ep.ConfigureConsumer<NewSubscriberConsumer>(provider);
                        EndpointConvention.Map<NewSubscriber>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint(host, "add-twitch-community-subscription", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));

                        ep.ConfigureConsumer<NewCommunitySubscriptionConsumer>(provider);
                        EndpointConvention.Map<NewCommunitySubscription>(ep.InputAddress);
                    });

                    cfg.ReceiveEndpoint(host, "add-twitch-user-ban", ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));

                        ep.ConfigureConsumer<NewBanConsumer>(provider);
                        EndpointConvention.Map<NewBan>(ep.InputAddress);
                    });

                    cfg.UseSerilog();
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
