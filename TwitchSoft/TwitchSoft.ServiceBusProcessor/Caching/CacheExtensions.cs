using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TwitchSoft.ServiceBusProcessor.Caching
{
    internal static class CacheExtensions
    {
        internal static void AddCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<IChannelsCache, ChannelsCache>();
        }
    }
}
