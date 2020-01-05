using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace TwitchSoft.Shared.Redis
{
    public static class RedisExtensions
    {
        public static void AddLocalRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisHost = configuration.GetValue<string>("Redis:Host");
            IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisHost);
            services.AddScoped(s => redis.GetDatabase());
        }
    }
}
