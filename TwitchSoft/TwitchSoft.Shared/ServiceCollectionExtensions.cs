using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.ElasticSearch;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;

namespace TwitchSoft.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureShared(this IServiceCollection services, IConfiguration configuration)
        {
            SqlMapper.Settings.CommandTimeout = 120; //override execution timeout for all queries

            SqlMapper.AddTypeMap(typeof(uint), System.Data.DbType.Int64);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserBanRepository, UserBanRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionStatisticRepository, SubscriptionStatisticRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<ITwitchApiService, TwitchApiService>();

            services.AddElasticSearch(configuration);

            services.AddCache(configuration);

            services
                .Configure<Services.Models.Telegram.BotSettings>(configuration.GetSection($"Telegram:{nameof(Services.Models.Telegram.BotSettings)}"))
                .Configure<Services.Models.Twitch.BotSettings>(configuration.GetSection($"Twitch:{nameof(Services.Models.Twitch.BotSettings)}"))
                .AddOptions();
        }
    }
}
