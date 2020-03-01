﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using TwitchSoft.Shared.ServiceBus.Models;
using TwitchSoft.Shared.ServiceBus.Configuration;
using MassTransit.Context;
using MassTransit.Azure.ServiceBus.Core;
using System;
using Microsoft.Azure.ServiceBus.Primitives;

namespace TwitchSoft.TwitchBot
{
    public static class ConfigurationExtensions
    {
        public static void AddServiceBusProcessors(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddMassTransit(x =>
            {
                LogContext.ConfigureCurrentLogContext();

                var serviceBusSettings = new ServiceBusSettings();
                Configuration.GetSection(nameof(ServiceBusSettings)).Bind(serviceBusSettings);

                x.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    var host = cfg.Host(serviceBusSettings.ConnectionString);

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
        }
    }
}
