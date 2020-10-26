using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TwitchSoft.TwitchBot
{
    public class OrcherstratorClient: IHostedService
    {
        private const string JoinChannelsCommand = "JoinChannelsCommand";

        private readonly HubConnection connection;
        private readonly ILogger<OrcherstratorClient> logger;
        private readonly TwitchBot twitchBot;

        public OrcherstratorClient(
            ILogger<OrcherstratorClient> logger, 
            IConfiguration configuration,
            TwitchBot twitchBot)
        {
            connection = new HubConnectionBuilder()
                .WithUrl(configuration.GetValue<string>("Services:TwitchBotOrchestratorHub"))
                .WithAutomaticReconnect()
                .Build();

            connection.On<IEnumerable<string>>(JoinChannelsCommand, channels => RefreshJoinedChannels(channels));

            connection.Reconnected += Connection_Reconnected;

            connection.Closed += Connection_Closed;

            this.logger = logger;
            this.twitchBot = twitchBot;
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

        public void RefreshJoinedChannels(IEnumerable<string> channels)
        {
            logger.LogInformation($"HubEvent: RefreshJoinedChannels triggered");
            twitchBot.RefreshJoinedChannels(channels);
        }
    }
}
