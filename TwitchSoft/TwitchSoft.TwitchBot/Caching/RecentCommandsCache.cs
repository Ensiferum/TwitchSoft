using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace TwitchSoft.TwitchBot.Caching
{
    public class RecentCommandsCache : IRecentCommandsCache
    {
        private const int CacheTimeInSeconds = 60;

        private readonly ILogger<RecentCommandsCache> logger;
        private readonly IMemoryCache memoryCache;

        public RecentCommandsCache(
            ILogger<RecentCommandsCache> logger,
            IMemoryCache memoryCache)
        {
            this.logger = logger;
            this.memoryCache = memoryCache;
        }

        public int GetAndUpdateCommandOccurences(string command)
        {
            if (memoryCache.TryGetValue(command, out int occuranceNumber))
            {
                occuranceNumber++;
                memoryCache.Set(command, occuranceNumber, MemoryCacheEntryOptions);
            }
            else
            {
                occuranceNumber = 1;
                memoryCache.Set(command, occuranceNumber, MemoryCacheEntryOptions);
            }

            logger.LogTrace($"Occurance {occuranceNumber} of command {command} in cache");
            return occuranceNumber;
        }

        public void DeleteCommand(string command)
        {
            logger.LogTrace($"Remove command {command} from cache");
            memoryCache.Remove(command);
        }

        MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions()
            // Keep in cache for this time, reset time if accessed.
            .SetSlidingExpiration(TimeSpan.FromSeconds(CacheTimeInSeconds));
    }
}
