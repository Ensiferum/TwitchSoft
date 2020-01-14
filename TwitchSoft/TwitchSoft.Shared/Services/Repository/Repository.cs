using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using UserTwitch = TwitchLib.Api.Helix.Models.Users.User;

namespace TwitchSoft.Shared.Services.Repository
{
    public class Repository : IRepository
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<Repository> logger;

        private string ConnectionString => configuration.GetConnectionString("TwitchDb");

        public Repository(
            IConfiguration configuration, 
            ILogger<Repository> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<Dictionary<string, uint>> GetUserIds(params string[] userNames)
        {
            using(var connection = new SqlConnection(ConnectionString))
            {
                var result = await connection.QueryAsync<(uint Id, string Username)>(@"
SELECT Id, Username FROM Users
WHERE Username IN @userNames", new { userNames });

                return result.ToDictionary(_ => _.Username, _ => _.Id);
            }
        }

        public async Task<IEnumerable<(uint Id, string Username)>> SearchUsers(string userNamePart, int count = 10)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryAsync<(uint Id, string Username)>(@"
SELECT TOP (@count) Id, Username FROM Users
WHERE Username LIKE @userNamePart
ORDER BY Id", new { userNamePart = $"%{userNamePart}%", count });
            }
        }

        public async Task<IEnumerable<User>> GetUsersByIds(IEnumerable<uint> ids)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryAsync<User>(@"
SELECT * FROM Users
WHERE Id IN @ids", new { ids });
            }
        }

        public async Task CreateOrUpdateUsers(params User[] users)
        {
            if (!users.Any())
            {
                return;
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                SqlTransaction trans = connection.BeginTransaction();

                await connection.ExecuteAsync(@$"
CREATE TABLE #TempUsers (Id bigint, Username nvarchar(60), JoinChannel bit, TrackMessages bit) 
", transaction: trans);

                await connection.ExecuteAsync(@$"
INSERT INTO #TempUsers (Id, Username, JoinChannel, TrackMessages) VALUES (@Id, @Username, @JoinChannel, @TrackMessages)
", users, trans);

                await connection.ExecuteAsync(@$"
MERGE Users us
USING #TempUsers tus
ON us.Id = tus.Id
WHEN MATCHED THEN
    UPDATE 
    SET us.Username = tus.Username, 
        us.JoinChannel = tus.JoinChannel, 
        us.TrackMessages = tus.TrackMessages
WHEN NOT MATCHED THEN
    INSERT (Id, Username, JoinChannel, TrackMessages)
    VALUES (tus.Id, tus.Username, tus.JoinChannel, tus.TrackMessages);
", transaction: trans);

                await trans.CommitAsync();
            }
        }

        public async Task SaveSubscriberAsync(params Subscription[] subscriptions)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.InsertAsync(subscriptions);
            }
        }

        public async Task SaveCommunitySubscribtionAsync(CommunitySubscription communitySubscription)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.InsertAsync(communitySubscription);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Error while saving {nameof(CommunitySubscription)}");
            }
            
        }

        public async Task SaveUserBansAsync(params UserBan[] userBans)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    await connection.InsertAsync(userBans);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while saving {nameof(UserBan)}");
            }
        }

        public async Task<IEnumerable<User>> GetChannelsToTrack()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryAsync<User>(@"
SELECT * FROM Users
WHERE JoinChannel = 1
");
            }
        }

        public async Task<bool> AddChannelToTrack(UserTwitch channel)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var user = await connection.GetAsync<User>(Int64.Parse(channel.Id));
                var userIsTracking = user?.JoinChannel == true;
                if (user == null)
                {
                    await connection.InsertAsync(new User
                    {
                        Id = uint.Parse(channel.Id),
                        Username = channel.Login,
                        JoinChannel = true,
                        TrackMessages = true,
                    });
                }
                else
                {
                    
                    user.JoinChannel = true;
                    user.TrackMessages = true;
                    await connection.UpdateAsync(user);
                }
                return userIsTracking;
            }
        }

        public async Task<IEnumerable<ChannelSubs>> GetTopChannelsBySubscribers(int skip, int count)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryAsync<ChannelSubs>(@"
SELECT us.Username as Channel, COUNT(*) AS SubsCount FROM Subscriptions sub
JOIN Users us ON sub.ChannelId = us.Id
WHERE sub.SubscribedTime >= @date
GROUP BY us.Username
ORDER BY SubsCount DESC
OFFSET @skip ROWS
FETCH NEXT @count ROWS ONLY
", new { count, skip, date = DateTime.UtcNow.AddMonths(-1) });
            }
        }

        public async Task<int> GetSubscribersCountFor(string channel)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.ExecuteScalarAsync<int>(@"
SELECT COUNT(*) FROM Subscriptions sub
JOIN Users us ON sub.ChannelId = us.Id
WHERE us.Username = @channel AND sub.SubscribedTime >= @date
", new { channel, date = DateTime.UtcNow.AddMonths(-1) });
            }
        }
    }
}
