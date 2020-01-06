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
            services.AddSingleton<IRedisClientsManager>(c => new RedisManagerPool(redisConnectionString));
            services.AddSingleton(c => c.GetService<IRedisClientsManager>().GetCacheClient());
            services.AddSingleton(c => c.GetService<IRedisClientsManager>().GetClient());
            services.AddSingleton<IChannelsCache, ChannelsCache>();
        }
    }
}
