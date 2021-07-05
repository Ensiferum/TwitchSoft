using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchSoft.TwitchBot.Caching
{
    internal static class CacheExtensions
    {
        internal static void AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IRecentCommandsCache, RecentCommandsCache>();
        }
    }
}
