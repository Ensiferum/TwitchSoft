using Coravel.Invocable;
using Microsoft.Extensions.Logging;
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
        private readonly IUserRepository userRepository;

        public EnsthorFollowsJoin(
            ILogger<EnsthorFollowsJoin> logger,
            ITwitchApiService twitchApiService,
            IUserRepository userRepository)
        {
            this.logger = logger;
            this.twitchApiService = twitchApiService;
            this.userRepository = userRepository;
        }
        public async Task Invoke()
        {
            string fromId = "74812507"; //ensthor
            logger.LogInformation($"Start executing job: {nameof(EnsthorFollowsJoin)}");
            var follows = await twitchApiService.GetFollowsForUser(fromId, null); 

            logger?.LogInformation($"Follows for {fromId}. Channels: {string.Join(", ", follows.Select(_ => _.ToUserName.ToLower()))}");

            await userRepository.CreateOrUpdateUsers(follows.Select(f => new User
            {
                Id = uint.Parse(f.ToUserId),
                Username = f.ToUserName.ToLower(),
                JoinChannel = true,
            }).ToArray());

            logger.LogInformation($"End executing job: {nameof(EnsthorFollowsJoin)}");
        }
    }
}
