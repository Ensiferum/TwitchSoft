using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Extensions;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared.Services.Helpers
{
    public class ChannelsCache : IChannelsCache
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<ChannelsCache> logger;
        private readonly IDatabase redisKeyDb;
        private readonly IDatabase redisNamesDb;

        public ChannelsCache(IServiceScopeFactory scopeFactory, ILogger<ChannelsCache> logger, ConnectionMultiplexer redisClient)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            redisKeyDb = redisClient.GetDatabase(0);
            redisNamesDb = redisClient.GetDatabase(1);
        }

        private string GetIdKey(uint channelId) => $"[id]{channelId}";
        private string GetNameKey(string channelName) => $"[name]{channelName}";

        public Task<List<User>> GetTrackedChannels()
        {
            return GetCachedChannels();
        }

        public async Task<string> GetChannelNameById(uint channelId)
        {
            var result = redisNamesDb.StringGet(GetIdKey(channelId));
            if (result.IsNull)
            {
                var channels = await GetCachedChannels();
                return channels.First(_ => _.Id == channelId).Username;
            }
            return result;
        }

        public async Task<uint> GetChannelIdByName(string channelName)
        {
            var channel = channelName.ToLower();
            var result = redisKeyDb.StringGet(GetNameKey(channel));
            if (result.IsNull)
            {
                var channels = await GetCachedChannels();
                return channels.First(_ => string.Equals(_.Username, channel)).Id;
            }
            return uint.Parse(result);
        }

        private Task<List<User>> GetCachedChannels()
        {
            return GetOrCreate(() => GetChannels());
        }

        private async Task<List<User>> GetChannels()
        {
            List<User> channels = null;
            await scopeFactory.RunInScope(async (scope) =>
            {
                var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
                channels = await repository.GetChannelsToTrack();

            });
            return channels;
        }

        private async Task<List<User>> GetOrCreate(Func<Task<List<User>>> createItem)
        {
            List<User> cacheEntry = await createItem();
            var cacheItems = new Dictionary<string, string>();
            var tasks = new List<Task>();
            var keysBatch = redisKeyDb.CreateBatch();
            var namesBatch = redisNamesDb.CreateBatch();
            cacheEntry.ForEach(user =>
            {
                tasks.Add(keysBatch.StringSetAsync(GetIdKey(user.Id), user.Username));
                tasks.Add(namesBatch.StringSetAsync(GetNameKey(user.Username.ToLower()), user.Id));
            });
            keysBatch.Execute();
            namesBatch.Execute();
            await Task.WhenAll(tasks);
            return cacheEntry;
        }
    }
}
