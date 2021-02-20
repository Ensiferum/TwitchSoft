using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class ChannelsBanRefresher : IInvocable
    {
        private readonly ILogger<ChannelsBanRefresher> logger;
        private readonly IUserRepository userRepository;

        public ChannelsBanRefresher(
            ILogger<ChannelsBanRefresher> logger, 
            IUserRepository userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
        }

        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(ChannelsBanRefresher)}");

            var channels = await userRepository.GetBannedChannels();

            foreach (var bannedChannel in channels)
            {
                await userRepository.SetChannelIsBanned(bannedChannel.Id, false);
            }

            logger.LogInformation($"End executing job: {nameof(ChannelsBanRefresher)}");
        }
    }
}
