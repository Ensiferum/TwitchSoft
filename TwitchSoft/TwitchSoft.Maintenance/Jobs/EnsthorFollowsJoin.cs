using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;
using User = TwitchSoft.Shared.Database.Models.User;

namespace TwitchSoft.Maintenance.Jobs
{
    public class EnsthorFollowsJoin : IInvocable
    {
        private readonly ILogger<EnsthorFollowsJoin> logger;
        private readonly ITwitchApiService twitchApiService;
        private readonly IRepository repository;

        public EnsthorFollowsJoin(
            ILogger<EnsthorFollowsJoin> logger,
            ITwitchApiService twitchApiService,
            IRepository repository)
        {
            this.logger = logger;
            this.twitchApiService = twitchApiService;
            this.repository = repository;
        }
        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(EnsthorFollowsJoin)}");
            var follows = await twitchApiService.GetFollowsForUser("74812507", null); //ensthor

            await repository.CreateOrUpdateUsers(follows.Select(f => new User
            {
                Id = uint.Parse(f.ToUserId),
                Username = f.ToUserName.ToLower(),
                JoinChannel = true,
                TrackMessages = true,
            }).ToArray());

            logger.LogInformation($"End executing job: {nameof(EnsthorFollowsJoin)}");
        }
    }
}
