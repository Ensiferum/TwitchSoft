using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Database;
using TwitchSoft.Shared.Services.TwitchApi;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.Maintenance.Jobs
{
    public class HoneymadFollowsJoin : IInvocable
    {
        private readonly ILogger<HoneymadFollowsJoin> logger;
        private readonly ITwitchApiService twitchApiService;
        private readonly TwitchDbContext twitchDbContext;

        public HoneymadFollowsJoin(
            ILogger<HoneymadFollowsJoin> logger,
            ITwitchApiService twitchApiService,
            TwitchDbContext twitchDbContext)
        {
            this.logger = logger;
            this.twitchApiService = twitchApiService;
            this.twitchDbContext = twitchDbContext;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(HoneymadFollowsJoin)}");
            var follows = await twitchApiService.GetFollowsForUser("40298003", null); //honeymad

            var followIds = follows.Select(_ => uint.Parse(_.ToUserId));

            var existingUsers = await twitchDbContext.Users.Where(_ => followIds.Contains(_.Id)).ToListAsync();
            existingUsers.ForEach(_ =>
            {
                _.Username = _.Username.ToLower();
                _.JoinChannel = true;
                _.TrackMessages = true;
            });

            twitchDbContext.Users.UpdateRange(existingUsers);

            var notExistingUsers = follows.Where(_ => !existingUsers.Select(u => u.Id).Contains(uint.Parse(_.ToUserId)));


            await twitchDbContext.Users.AddRangeAsync(notExistingUsers.Select(f => new User
            {
                Id = uint.Parse(f.ToUserId),
                Username = f.ToUserName.ToLower(),
                JoinChannel = true,
                TrackMessages = true,
            }));
            await twitchDbContext.SaveChangesAsync();

            logger.LogInformation($"End executing job: {nameof(HoneymadFollowsJoin)}");
        }
    }
}
