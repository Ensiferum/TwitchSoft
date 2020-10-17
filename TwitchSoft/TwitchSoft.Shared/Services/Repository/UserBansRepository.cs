using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared.Services.Repository
{
    public class UserBansRepository : BaseRepository, IUserBansRepository
    {
        public UserBansRepository(IConfiguration configuration, ILogger<BaseRepository> logger) : base(configuration, logger)
        {
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
    }
}
