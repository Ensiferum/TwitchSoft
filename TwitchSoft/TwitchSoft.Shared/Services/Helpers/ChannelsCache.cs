using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared.Services.Helpers
{
    public class ChannelsCache : IChannelsCache
    {
        private readonly ILogger<ChannelsCache> logger;
        private readonly IMemoryCache memoryCache;
        private readonly IRepository repository;

        public ChannelsCache(
            ILogger<ChannelsCache> logger,
            IMemoryCache memoryCache,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.repository = serviceProvider.GetService<IRepository>();
        }

        public async Task<string> GetChannelNameById(uint channelId)
        {
            if (memoryCache.TryGetValue(channelId, out string value))
            {
                return value;
            }
            else
            {
                logger.LogInformation("Missing channelId in cache", channelId);
                var user = await repository.GetUserById(channelId);
                memoryCache.Set(channelId, user.Username);
                return user.Username;
            }
        }

        public async Task<uint> GetChannelIdByName(string channelName)
        {
            if (memoryCache.TryGetValue(channelName, out uint value))
            {
                return value;
            }
            else
            {
                logger.LogInformation("Missing channelName in cache", channelName);
                var user = await repository.GetUserByName(channelName);
                memoryCache.Set(channelName, user.Id);
                return user.Id;
            }
        }
    }
}
