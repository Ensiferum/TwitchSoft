using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchSoft.TwitchBot.Caching
{
    internal static class CacheExtensions
    {
        internal static void AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSingleton<IRecentCommandsCache, RecentCommandsCache>();
        }
    }
}
