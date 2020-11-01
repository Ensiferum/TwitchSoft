using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using UserTwitch = TwitchLib.Api.Helix.Models.Users.User;

namespace TwitchSoft.Shared.Services.Repository
{
    public class UsersRepository : BaseRepository, IUsersRepository
    {
        public UsersRepository(IConfiguration configuration, ILogger<BaseRepository> logger) : base(configuration, logger)
        {
        }

        public async Task<Dictionary<string, uint>> GetUserIds(params string[] userNames)
        {
            using (var connection = new SqlConnection(ConnectionString))
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
WHERE Username = @userNamePart
ORDER BY Id", new { userNamePart, count });
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

            if (users.Length == 1)
            {
                await CreateOrUpdateUser(users[0]);
                return;
            }

            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                SqlTransaction trans = connection.BeginTransaction();

                await connection.ExecuteAsync(@$"
CREATE TABLE #TempUsers (Id bigint, Username nvarchar(60), JoinChannel bit) 
", transaction: trans);

                await connection.ExecuteAsync(@$"
INSERT INTO #TempUsers (Id, Username, JoinChannel) VALUES (@Id, @Username, @JoinChannel)
", users, trans);

                await connection.ExecuteAsync(@$"
MERGE Users us
USING #TempUsers tus
ON us.Id = tus.Id
WHEN MATCHED THEN
    UPDATE 
    SET us.Username = tus.Username, 
        us.JoinChannel = IIF(us.JoinChannel = 1, us.JoinChannel, tus.JoinChannel)
WHEN NOT MATCHED THEN
    INSERT (Id, Username, JoinChannel)
    VALUES (tus.Id, tus.Username, tus.JoinChannel);
", transaction: trans);

                await trans.CommitAsync();
            }
        }

        public async Task CreateOrUpdateUser(User user)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(@$"MERGE INTO Users
                USING 
                (
                   SELECT   {user.Id} as Id,
                            '{user.Username}' AS Username
                ) AS entity
                ON  Users.Id = entity.Id
                WHEN MATCHED THEN
                    UPDATE 
                    SET Username = '{user.Username}'
                WHEN NOT MATCHED THEN
                    INSERT (Id, Username)
                    VALUES ({user.Id}, '{user.Username}');");
            }
        }

        public async Task SetChannelIsBanned(string channelName, bool isBanned)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                await connection.ExecuteAsync(@"
UPDATE Users
SET IsBanned = @isBanned
WHERE Username = @channelName
", new { channelName, isBanned });
            }
        }

        public async Task<User> GetUserById(uint id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<User>(@"
SELECT * FROM Users
WHERE Id = @id
", new { id });
            }
        }

        public async Task<User> GetUserByName(string name)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<User>(@"
SELECT * FROM Users
WHERE Username = @name
", new { name });
            }
        }

        public async Task<IEnumerable<User>> GetChannelsToTrack()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return await connection.QueryAsync<User>(@"
SELECT * FROM Users
WHERE JoinChannel = 1 AND IsBanned = 0
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
                    });
                }
                else
                {

                    user.JoinChannel = true;
                    await connection.UpdateAsync(user);
                }
                return userIsTracking;
            }
        }

        
    }
}
