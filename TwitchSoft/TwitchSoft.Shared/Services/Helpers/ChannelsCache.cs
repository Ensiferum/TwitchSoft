﻿using Microsoft.Extensions.Caching.Memory;
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
        private readonly IUsersRepository usersRepository;

        public ChannelsCache(
            ILogger<ChannelsCache> logger,
            IMemoryCache memoryCache,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.usersRepository = serviceProvider.GetService<IUsersRepository>();
        }

        public async Task<string> GetChannelNameById(uint channelId)
        {
            if (memoryCache.TryGetValue(channelId, out string value))
            {
                return value;
            }
            else
            {
                logger.LogTrace($"Missing channelId {channelId} in cache");
                var user = await usersRepository.GetUserById(channelId);
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
                logger.LogTrace($"Missing channelName {channelName} in cache");
                var user = await usersRepository.GetUserByName(channelName);
                memoryCache.Set(channelName, user.Id);
                return user.Id;
            }
        }
    }
}
