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

        public Task<uint> GetUserId(string userName)
        {
            return twitchDbContext.Users.Where(_ => _.Username == userName).Select(_ => _.Id).FirstOrDefaultAsync();
        }

        public Task<List<User>> SearchUsers(string userNamePart, int count = 10)
        {
            return twitchDbContext.Users.Where(_ => _.Username.Contains(userNamePart)).Take(count).ToListAsync();
        }

        public Task CreateOrUpdateUser(User user)
        {
            return twitchDbContext.Database.ExecuteSqlRawAsync(@"MERGE INTO Users
                USING 
                (
                   SELECT   {0} as Id,
                            {1} AS Username
                ) AS entity
                ON  Users.Id = entity.Id
                WHEN MATCHED THEN
                    UPDATE 
                    SET Username = {1}
                WHEN NOT MATCHED THEN
                    INSERT (Id, Username)
                    VALUES ({0}, {1});", user.Id, user.Username);
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

        public async Task SaveUserBanAsync(UserBan userBan)
        {
            try
            {
                twitchDbContext.UserBans.Add(userBan);
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
