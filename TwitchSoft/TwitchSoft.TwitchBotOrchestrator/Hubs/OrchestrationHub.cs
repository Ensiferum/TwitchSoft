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
    public class OrchestrationHub: Hub
    {
        private static ConcurrentDictionary<string, List<string>> ConnectionChannelList = new ConcurrentDictionary<string, List<string>>();
        private static DebounceDispatcher DebounceDispatcher = new DebounceDispatcher(10000);

        private readonly IRepository repository;
        private readonly ILogger<OrchestrationHub> logger;

        public OrchestrationHub(
            IRepository repository,
            ILogger<OrchestrationHub> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> array, int size)
        {
            for (var i = 0; i < (float)(array.Count() / size); i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public async Task TriggerReconnect()
        {
            var channels = await repository.GetChannelsToTrack();
            var botsCount = ConnectionChannelList.Count;
            var channelGroups = Split(channels.Select(_ => _.Username), botsCount);

            logger.LogInformation("Hub Clients connected", botsCount);

            for (int i = 0; i < botsCount; i++)
            {
                var botConnectionInfo = ConnectionChannelList.ElementAt(i);
                botConnectionInfo.Value.Clear();
                var channelsToJoin = channelGroups.ElementAt(i);
                botConnectionInfo.Value.AddRange(channelsToJoin);

                logger.LogInformation("Client channels", botConnectionInfo.Key, botConnectionInfo.Value);

                await Clients.Clients(botConnectionInfo.Key).SendAsync("joinchannels", channelsToJoin);
            }
        }

        public override async Task OnConnectedAsync()
        {
            logger.LogInformation("Hub client connected", Context.ConnectionId);
            await DebounceDispatcher.DebounceAsync(TriggerReconnect);
            ConnectionChannelList.TryAdd(Context.ConnectionId, new List<string>());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            logger.LogInformation("Hub client disconnected", Context.ConnectionId);
            await DebounceDispatcher.DebounceAsync(TriggerReconnect);
            ConnectionChannelList.TryRemove(Context.ConnectionId, out _);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
