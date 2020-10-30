using DebounceThrottle;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.TwitchBotOrchestrator.Hubs
{
    public class OrchestrationHub: Hub<IOrchestrationClient>
    {
        private static ConcurrentDictionary<string, List<string>> ConnectionChannelList = new ConcurrentDictionary<string, List<string>>();
        private static DebounceDispatcher DebounceDispatcher = new DebounceDispatcher(10000);

        private readonly IUsersRepository usersRepository;
        private readonly ILogger<OrchestrationHub> logger;

        public OrchestrationHub(
            IUsersRepository usersRepository,
            ILogger<OrchestrationHub> logger)
        {
            this.usersRepository = usersRepository;
            this.logger = logger;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> array, int numOfParts)
        {
            int i = 0;
            return array.GroupBy(x => i++ % numOfParts);
        }

        public static async Task AddChannel(IHubClients<IOrchestrationClient> clients, string channelname, ILogger logger)
        {
            logger.LogInformation($"AddChannel: Channel {channelname}");
            if (ConnectionChannelList.Any())
            {
                if (!ConnectionChannelList.Values.SelectMany(_ => _).Contains(channelname, StringComparer.OrdinalIgnoreCase))
                {
                    var clientToConnect = ConnectionChannelList.OrderByDescending(_ => _.Value.Count).First();
                    clientToConnect.Value.Add(channelname);
                    await clients.Client(clientToConnect.Key).JoinChannelsCommand(clientToConnect.Value);
                }
            }
            else
            {
                logger.LogInformation("No active bots connected");
            }
        }

        public async Task TriggerReconnect()
        {
            var channels = await usersRepository.GetChannelsToTrack();
            await JoinChannelsCommand(channels.Select(_ => _.Username));
        }

        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("Hub client connected", Context.ConnectionId);
            ConnectionChannelList.TryAdd(Context.ConnectionId, new List<string>());
            await DebounceDispatcher.DebounceAsync(TriggerReconnect);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            logger.LogInformation("Hub client disconnected", Context.ConnectionId);
            ConnectionChannelList.TryRemove(Context.ConnectionId, out _);
            await DebounceDispatcher.DebounceAsync(TriggerReconnect);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChannelsCommand(IEnumerable<string> channels)
        {
            await RefreshChannels(Clients, channels, logger);
        }

        public static async Task RefreshChannels(IHubClients<IOrchestrationClient> clients, IEnumerable<string> channels, ILogger logger)
        {
            var botsCount = ConnectionChannelList.Count;
            var channelGroups = Split(channels, botsCount);

            logger?.LogInformation($"Hub Clients connected: {botsCount}");

            for (int i = 0; i < botsCount; i++)
            {
                var botConnectionInfo = ConnectionChannelList.ElementAt(i);
                botConnectionInfo.Value.Clear();
                var channelsToJoin = channelGroups.ElementAt(i);
                botConnectionInfo.Value.AddRange(channelsToJoin);

                logger?.LogInformation($"Client channels. Client: {botConnectionInfo.Key}. Channels: {string.Join(", ", channelsToJoin)}");

                await clients.Client(botConnectionInfo.Key).JoinChannelsCommand(channelsToJoin);
            }
        }
    }
}
