using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Maintenance.Jobs
{
    public class ChannelsBanRefresher : IInvocable
    {
        private readonly ILogger<ChannelsBanRefresher> logger;
        private readonly IUsersRepository usersRepository;

        public ChannelsBanRefresher(
            ILogger<ChannelsBanRefresher> logger, 
            IUsersRepository usersRepository)
        {
            this.logger = logger;
            this.usersRepository = usersRepository;
        }

        public async Task Invoke()
        {
            logger.LogInformation($"Start executing job: {nameof(ChannelsBanRefresher)}");

            var channels = await usersRepository.GetBannedChannels();

            foreach (var bannedChannel in channels)
            {
                await usersRepository.SetChannelIsBanned(bannedChannel.Id, false);
            }

            logger.LogInformation($"End executing job: {nameof(ChannelsBanRefresher)}");
        }
    }
}
