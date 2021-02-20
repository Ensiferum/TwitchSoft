using Dapper;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureShared(this IServiceCollection services)
        {
            SqlMapper.Settings.CommandTimeout = 120; //override execution timeout for all queries

            SqlMapper.AddTypeMap(typeof(uint), System.Data.DbType.Int64);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserBanRepository, UserBanRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionStatisticRepository, SubscriptionStatisticRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
        }
    }
}
