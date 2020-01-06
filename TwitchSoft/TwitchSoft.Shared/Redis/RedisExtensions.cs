using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;
using TwitchSoft.Shared.Services.Helpers;

namespace TwitchSoft.Shared.Redis
{
    public static class RedisExtensions
    {
        public static void AddLocalRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString");
            var manager = new RedisManagerPool(redisConnectionString);
            services.AddSingleton<IRedisClientsManager>(c => manager);
            services.AddSingleton(c => manager.GetCacheClient());
            services.AddSingleton(c => manager.GetClient());
            services.AddSingleton<IChannelsCache, ChannelsCache>();
        }
    }
}
