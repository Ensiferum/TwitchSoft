using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceStack.Caching;
using ServiceStack.Redis;
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
        private readonly ICacheClient cacheClient;

        public ChannelsCache(IServiceScopeFactory scopeFactory, ILogger<ChannelsCache> logger, ICacheClient cacheClient)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.cacheClient = cacheClient;
        }

        private string GetIdKey(uint channelId) => $"[id]{channelId}";
        private string GetNameKey(string channelName) => $"[name]{channelName}";

        public Task<List<User>> GetTrackedChannels()
        {
            return GetCachedChannels();
        }

        public async Task<User> GetChannelById(uint channelId)
        {
            var result = cacheClient.Get<User>(GetIdKey(channelId));
            if (result == null)
            {
                var channels = await GetCachedChannels();
                return channels.FirstOrDefault(_ => _.Id == channelId);
            }
            return result;
        }

        public async Task<User> GetChannelByName(string channelName)
        {
            var channel = channelName.ToLower();
            var result = cacheClient.Get<User>(GetNameKey(channel));
            if (result == null)
            {
                var channels = await GetCachedChannels();
                return channels.FirstOrDefault(_ => string.Equals(_.Username, channel));
            }
            return result;
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
            var cacheItems = new Dictionary<string, User>();
            cacheEntry.ForEach(user =>
            {
                cacheItems.Add(GetIdKey(user.Id), user);
                cacheItems.Add(GetNameKey(user.Username.ToLower()), user);
            });
            cacheClient.SetAll<User>(cacheItems);
            return cacheEntry;
        }
    }
}
