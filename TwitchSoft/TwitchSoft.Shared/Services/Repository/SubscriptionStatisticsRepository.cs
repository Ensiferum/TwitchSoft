using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared.Services.Repository
{
    public class SubscriptionStatisticsRepository : BaseRepository, ISubscriptionStatisticsRepository
    {
        public SubscriptionStatisticsRepository(IConfiguration configuration, ILogger<BaseRepository> logger) : base(configuration, logger)
        {
        }

        public async Task SaveStatistic(SubscriptionStatistic subStat)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.InsertAsync(subStat);
        }
    }
}
