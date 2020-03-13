using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TwitchSoft.Shared.Services.Helpers;

namespace TwitchSoft.Shared.Redis
{
    public static class CacheExtensions
    {
        public static void AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString");

            var redis = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(redis);
            services.AddMemoryCache();
            services.AddSingleton<IChannelsCache, ChannelsCache>();
        }
    }
}
