using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared.Services.Repository
{
    public class SubscriptionRepository : BaseRepository, ISubscriptionRepository
    {
        public SubscriptionRepository(IConfiguration configuration, ILogger<BaseRepository> logger) : base(configuration, logger)
        {
        }

        public async Task SaveSubscriberAsync(params Subscription[] subscriptions)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.InsertAsync(subscriptions);
        }

        public async Task SaveCommunitySubscribtionAsync(CommunitySubscription communitySubscription)
        {
            try
            {
                using var connection = new SqlConnection(ConnectionString);
                await connection.InsertAsync(communitySubscription);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while saving {nameof(CommunitySubscription)}");
            }
        }

        public async Task<IEnumerable<ChannelSubs>> GetSubscribersCount(int skip, int count, DateTime? fromDate = null)
        {
            fromDate ??= DateTime.UtcNow.AddMonths(-1);
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<ChannelSubs>(@"
SELECT us.Username as Channel, COUNT(*) AS SubsCount FROM Subscriptions sub
JOIN Users us ON sub.ChannelId = us.Id
WHERE sub.SubscribedTime >= @date
GROUP BY us.Username
ORDER BY SubsCount DESC
OFFSET @skip ROWS
FETCH NEXT @count ROWS ONLY
", new { count, skip, date = fromDate });
        }

        public async Task<int> GetMonthlySubscribersCountFor(string channel)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.ExecuteScalarAsync<int>(@"
SELECT COUNT(*) FROM Subscriptions sub
JOIN Users us ON sub.ChannelId = us.Id
WHERE us.Username = @channel AND sub.SubscribedTime >= @date
", new { channel, date = DateTime.UtcNow.AddMonths(-1) });
        }

        public async Task<int> GetSubscribersCountFor(string channel, DateTime from, DateTime to)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.ExecuteScalarAsync<int>(@"
SELECT COUNT(*) FROM Subscriptions sub
JOIN Users us ON sub.ChannelId = us.Id
WHERE us.Username = @channel AND sub.SubscribedTime >= @from AND sub.SubscribedTime < @to
", new { channel, from, to });
        }

        public async Task<int> GetSubscribersCountOnDay(uint channelId, DateTime date)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.ExecuteScalarAsync<int>(@"
SELECT COUNT(*) FROM Subscriptions sub
WHERE sub.ChannelId = @channelId AND sub.SubscribedTime BETWEEN @from AND @to
", new { channelId, from = date, to = date.AddDays(1).AddMilliseconds(-1) });
        }
    }
}
