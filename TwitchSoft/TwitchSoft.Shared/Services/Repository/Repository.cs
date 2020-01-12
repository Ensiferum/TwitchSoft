using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database;
using TwitchSoft.Shared.Database.Models;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using UserTwitch = TwitchLib.Api.Helix.Models.Users.User;

namespace TwitchSoft.Shared.Services.Repository
{
    public class Repository : IRepository
    {
        private readonly TwitchDbContext twitchDbContext;
        private readonly ILogger<Repository> logger;

        public Repository(
            TwitchDbContext twitchDbContext, 
            ILogger<Repository> logger)
        {
            this.twitchDbContext = twitchDbContext;
            this.logger = logger;
        }

        public Task<Dictionary<string, uint>> GetUserIds(params string[] userNames)
        {
            return twitchDbContext.Users
                .Where(_ => userNames.Contains(_.Username))
                .Select(_ => new { _.Id, _.Username })
                .ToDictionaryAsync(_ => _.Username, _ => _.Id);
        }

        public Task<List<User>> SearchUsers(string userNamePart, int count = 10)
        {
            return twitchDbContext.Users.Where(_ => _.Username.Contains(userNamePart)).Take(count).ToListAsync();
        }

        public Task CreateOrUpdateUsers(params User[] users)
        {
            if (!users.Any())
            {
                return Task.CompletedTask;
            }

            return twitchDbContext.Database.ExecuteSqlRawAsync(@$"
CREATE TABLE #TempUsers (Id bigint, Name nvarchar(60)) 

INSERT INTO #TempUsers VALUES
{string.Join(",", users.Select(_ => $"({_.Id}, {_.Username})"))}

MERGE Users us
USING #TempUsers tus
ON us.Id = tus.Id
WHEN MATCHED THEN
    UPDATE 
    SET us.Username = tus.Name
WHEN NOT MATCHED THEN
    INSERT (Id, Username)
    VALUES (tus.Id, tus.Name);
");
        }

        public async Task SaveSubscriberAsync(params Subscription[] subscriptions)
        {
            try
            {
                twitchDbContext.Subscriptions.AddRange(subscriptions);
                await twitchDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving");
            }
        }

        public async Task SaveCommunitySubscribtionAsync(CommunitySubscription communitySubscription)
        {
            try
            {
                twitchDbContext.CommunitySubscriptions.Add(communitySubscription);
                await twitchDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving");
            }
        }

        public async Task SaveUserBansAsync(params UserBan[] userBans)
        {
            try
            {
                twitchDbContext.UserBans.AddRange(userBans);
                await twitchDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving");
            }
        }

        public async Task<List<User>> GetChannelsToTrack()
        {
            return await twitchDbContext.Users.Where(_ => _.JoinChannel).ToListAsync();
        }

        public async Task<bool> AddChannelToTrack(UserTwitch channel)
        {
            var user = await twitchDbContext.Users.FirstOrDefaultAsync(_ => _.Id == uint.Parse(channel.Id));
            var userIsTracking = user?.JoinChannel == true;
            if (user == null)
            {
                twitchDbContext.Users.Add(new User
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
                twitchDbContext.Users.Update(user);
            }
            await twitchDbContext.SaveChangesAsync();
            return userIsTracking;
        }

        public Task<List<ChannelSubs>> GetTopChannelsBySubscribers(int skip, int count)
        {
            return twitchDbContext.Subscriptions
                .Where(_ => _.SubscribedTime >= DateTime.UtcNow.AddMonths(-1))
                .GroupBy(_ => _.Channel.Username)
                .OrderByDescending(g => g.Count())
                .Select(g => new ChannelSubs
                {
                    Channel = g.Key,
                    SubsCount = g.Count()
                })
                .Skip(skip)
                .Take(count)
                .ToListAsync();
        }

        public Task<int> GetSubscribersCountFor(string channel)
        {
            return twitchDbContext.Subscriptions
                .Where(_ => _.SubscribedTime >= DateTime.UtcNow.AddMonths(-1))
                .Where(_ => _.Channel.Username == channel)
                .CountAsync();
        }
    }
}
