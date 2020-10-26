﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;

namespace TwitchSoft.TwitchBot
{
    public class OrcherstratorClient: IHostedService
    {
        private const string JoinChannelsCommand = "JoinChannelsCommand";

        private readonly HubConnection connection;
        private readonly ILogger<OrcherstratorClient> logger;
        private readonly ITwitchClient twitchClient;

        public OrcherstratorClient(
            ILogger<OrcherstratorClient> logger, 
            IConfiguration configuration,
            ITwitchClient twitchClient)
        {
            connection = new HubConnectionBuilder()
                .WithUrl(configuration.GetValue<string>("Services:TwitchBotOrchestratorHub"))
                .WithAutomaticReconnect()
                .Build();

            connection.On<IEnumerable<string>>(JoinChannelsCommand, async channels => await RefreshJoinedChannels(channels));

            connection.Reconnected += Connection_Reconnected;

            connection.Closed += Connection_Closed;

            this.logger = logger;
            this.twitchClient = twitchClient;
        }

        private Task Connection_Closed(Exception arg)
        {
            logger.LogError(arg, "Hub Connection Closed");
            return Task.CompletedTask;
        }

        private Task Connection_Reconnected(string arg)
        {
            logger.LogInformation("Hub Connection Reconnected", arg);
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await connection.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured");
            }

            logger.LogInformation($"HubInfo: {connection.ConnectionId}, {connection.State}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return connection.StopAsync(cancellationToken);
        }

        public async Task RefreshJoinedChannels(IEnumerable<string> channels)
        {
            logger.LogInformation($"RefreshJoinedChannels triggered. Channels: {string.Join(", ", channels)}");

            if (twitchClient.IsConnected)
            {
                logger.LogInformation($"Twitch Client is connected");
                var joinedChannels = twitchClient.JoinedChannels;

                foreach (var channel in joinedChannels)
                {
                    if (channels.Any(_ => _.Equals(channel.Channel, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                    else
                    {
                        twitchClient.LeaveChannel(channel.Channel);
                    }
                }

                foreach (var channel in channels)
                {
                    if (joinedChannels.Any(_ => _.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                    else
                    {
                        twitchClient.JoinChannel(channel);
                    }
                }
            } else
            {
                logger.LogInformation($"Twitch Client is not connected. Delaying...");
                await Task.Delay(5000);
                await RefreshJoinedChannels(channels);
            }
        }
    }
}
