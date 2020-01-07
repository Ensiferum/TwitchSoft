using StackExchange.Redis;
using System.Text.Json;

namespace TwitchSoft.Shared.Services.Helpers
{
    public static class DistributedCacheExtensions
    {
        public static void Set<T>(this IDatabase db, string key, T value)
        {
            db.StringSet(key, JsonSerializer.Serialize<T>(value));
        }

        public static T Get<T>(this IDatabase db, string key)
        {
            var value = db.StringGet(key);

            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }
    }
}
