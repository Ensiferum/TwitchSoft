using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Extensions;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared.Services.Helpers
{
    public class ChannelsCache : IChannelsCache
    {
        private readonly string Key = "ChannelsCache";
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<ChannelsCache> logger;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        public ChannelsCache(IServiceScopeFactory scopeFactory, ILogger<ChannelsCache> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        public Task<List<User>> GetTrackedChannels()
        {
            return GetCachedChannels();
        }

        public async Task<User> GetChannelById(uint channelId)
        {
            var channels = await GetCachedChannels();
            return channels.FirstOrDefault(_ => _.Id == channelId);
        }

        public async Task<User> GetChannelByName(string channelName)
        {
            var channels = await GetCachedChannels();
            return channels.FirstOrDefault(_ => string.Equals(_.Username, channelName, StringComparison.OrdinalIgnoreCase));
        }

        public void InvalidateCache()
        {
            logger.LogInformation("Cache invalidated");
            _cache.Remove(Key);
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
            List<User> cacheEntry;
            if (!_cache.TryGetValue(Key, out cacheEntry))// Look for cache key.
            {
                SemaphoreSlim mylock = _locks.GetOrAdd(Key, k => new SemaphoreSlim(1, 1));

                await mylock.WaitAsync();
                try
                {
                    if (!_cache.TryGetValue(Key, out cacheEntry))
                    {
                        // Key not in cache, so get data.
                        cacheEntry = await createItem();
                        _cache.Set(Key, cacheEntry);
                    }
                }
                finally
                {
                    mylock.Release();
                }
            }
            return cacheEntry;
        }
    }
}
